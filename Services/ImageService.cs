using AppComposer.Models.Layers;
using OpenCvSharp;

namespace AppComposer.Services
{
    public class ImageService
    {
       
        public ImageService() 
        {
        
        }

        public ImageLayer Load(string filename, int canvasWidth, int canvasHeight)
        {
            Mat image = Cv2.ImRead(filename, ImreadModes.Color);

            Mat bwImage = ConvertTo1bpp(image);

            return new ImageLayer(filename, bwImage);
        }

        public Mat ConvertTo1bpp(Mat image)
        {
            Mat grayImage = new Mat();
            Mat bwImage = new Mat();
            Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(grayImage, bwImage, 128, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
            return bwImage;
        }
    }
}
