namespace QualityBot.Persistence
{
    using System;
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using QualityBot.MediaService;
    using QualityBot.Util;

    public class MediaServicePersister
    {
        private readonly Guid _applicationId;

        private string _mediaServiceEnvironment;

        private string _mediaServiceBackendEnvironment;

        private MediaStoreClient _mediaStoreClient;

        public MediaServicePersister()
        {
            _mediaServiceEnvironment = AppConfigUtil.AppConfig("mediaServiceEnvironment");
            _applicationId = new Guid(AppConfigUtil.AppConfig("applicationGuid"));
            _mediaServiceBackendEnvironment = AppConfigUtil.AppConfig("mediaServiceBackendEnvironment");
            var endpointAddress = new EndpointAddress(
                new Uri(string.Format(@"http://media.{0}.com/MediaStore.v1.http", _mediaServiceBackendEnvironment)));
            Binding bindingInfo = new BasicHttpBinding
            {
                MaxReceivedMessageSize = int.MaxValue,
                MaxBufferPoolSize = int.MaxValue,
                MaxBufferSize = int.MaxValue,
                MessageEncoding = WSMessageEncoding.Mtom,
                TransferMode = TransferMode.StreamedRequest
            };

            _mediaStoreClient = new MediaStoreClient(bindingInfo, endpointAddress);
        }

        public string SaveHtmlToMediaService(string html, string suffix, string extension)
        {
            byte[] bytes = new byte[html.Length * sizeof(char)];
            Buffer.BlockCopy(html.ToCharArray(), 0, bytes, 0, bytes.Length);
            var memoryStream = new MemoryStream(bytes);
            return SaveStreamToMediaService(memoryStream, suffix, extension, @"text/html");
        }

        public string SaveImageToMediaService(string base64Image, string suffix, string extension)
        {
            var image = ImageUtil.Base64ToBytes(base64Image);

            var memoryStream = new MemoryStream(image);

            return SaveStreamToMediaService(memoryStream, suffix, extension, @"image/png");
        }

        public string SaveStreamToMediaService(Stream stream, string suffix, string extension, string mimeType)
        {
            var saveName = string.Format(@"{0}_{1}.{2}", DateTime.Now.ToString("yyyyMMddHHmmssfffffff"), suffix, extension);

            var mediaBase = new MediaBase
            {
                FileName = saveName,
                FileSize = stream.Length,
                MediaOwner = _applicationId,
                Title = Path.GetFileNameWithoutExtension(saveName),
                FileType = FileType.Image
            };

            var guid = _mediaStoreClient.SaveFile(_applicationId, null, saveName, stream.Length, mediaBase, stream);

            return string.Format(@"http://m.{0}.com/File/{1}?mimeType={2}", _mediaServiceEnvironment, guid, mimeType);
        }
    }
}