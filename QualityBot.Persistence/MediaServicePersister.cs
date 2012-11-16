namespace QualityBot.Persistence
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Text;
    using QualityBot.Persistence.MediaService;
    using QualityBot.Util;

    public sealed class MediaServicePersister : IDisposable
    {
        private static Guid _applicationId;

        private static string _mediaServiceBackendEnvironment;

        private static string _mediaServiceEnvironment;

        private static dynamic _settings;

        private MediaStoreClient _mediaStoreClient;

        public MediaServicePersister()
        {
            Config();

            var endpointAddress = new EndpointAddress(new Uri(string.Format(@"http://media.{0}.com/MediaStore.v1.http", _mediaServiceBackendEnvironment)));

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

        public void Dispose()
        {
            _mediaStoreClient.Close();
        }

        public string SaveHtmlToMediaService(string html, string suffix, string extension)
        {
            var memoryStream = new MemoryStream(UTF8Encoding.Default.GetBytes(html));
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

        private static void Config()
        {
            if (_settings == null)
            {
                string result;
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("QualityBot.Persistence.mediaServiceSettings.json"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }

                _settings = SettingsUtil.ReadSettingsFromString(result);
            }

            if (string.IsNullOrWhiteSpace(_mediaServiceEnvironment))
            {
                _mediaServiceEnvironment = SettingsUtil.GetConfig("mediaServiceEnvironment", _settings);
            }

            if (_applicationId == default(Guid))
            {
                _applicationId = new Guid(SettingsUtil.GetConfig("applicationGuid", _settings));
            }

            if (string.IsNullOrWhiteSpace(_mediaServiceBackendEnvironment))
            {
                _mediaServiceBackendEnvironment = SettingsUtil.GetConfig("mediaServiceBackendEnvironment", _settings);
            }
        }
    }
}