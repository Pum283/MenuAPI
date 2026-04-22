using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Media
{
    public interface IMediaService
    {
        Task<(string Url, string PublicId)> UploadImageAsync(Stream stream, string fileName, string? overwritePublicId, CancellationToken cancellationToken = default);
        Task DeleteAsync(string publicId, CancellationToken cancellationToken = default);
    }

    public class CloudinaryMediaService : IMediaService
    {
        private readonly Cloudinary _cloudinary;
        private readonly CloudinaryOptions _options;

        public CloudinaryMediaService(IOptions<CloudinaryOptions> options)
        {
            _options = options.Value;
            _cloudinary = new Cloudinary(_options.CloudinaryUrl);
            _cloudinary.Api.Secure = true;
        }

        public async Task<(string Url, string PublicId)> UploadImageAsync(
            Stream stream,
            string fileName,
            string? overwritePublicId,
            CancellationToken cancellationToken = default)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
                Folder = _options.Folder,
                PublicId = overwritePublicId,
                Overwrite = !string.IsNullOrWhiteSpace(overwritePublicId),
                UniqueFilename = string.IsNullOrWhiteSpace(overwritePublicId),
            };

            var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);
            if (result.Error != null)
            {
                throw new InvalidOperationException(result.Error.Message);
            }

            return (result.SecureUrl.ToString(), result.PublicId);
        }

        public async Task DeleteAsync(string publicId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(publicId)) return;
            var delParams = new DeletionParams(publicId) { ResourceType = ResourceType.Image };
            // CloudinaryDotNet 1.x does not accept CancellationToken overload here.
            var result = await _cloudinary.DestroyAsync(delParams);
            if (result.Error != null)
            {
                throw new InvalidOperationException(result.Error.Message);
            }
        }
    }
}

