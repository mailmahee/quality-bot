namespace QualityBot.Util
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Mapper;

    public class SpriteUtil
    {
        public Sprite MapImagesToSprite(IEnumerable<Image> images)
        {
            var imageInfos = images.ToDictionary(i => i, i => new Rectangle(0, 0, i.Width, i.Height));
            var canvas = new Canvas();
            var mapper = new MapperOptimalEfficiency(canvas);
            var sprite = mapper.Mapping(imageInfos);

            return sprite;
        }

        public Bitmap DrawSprite(Sprite sprite)
        {
            var bitmap = new Bitmap(sprite.Width, sprite.Height);
			using (var g = Graphics.FromImage(bitmap))
			{
				foreach (var imageRect in sprite.MappedImages)
				{
					g.DrawImage(imageRect.Key, imageRect.Value.X, imageRect.Value.Y);
				}

				g.Save();
			}

            return bitmap;
        }
    }
}