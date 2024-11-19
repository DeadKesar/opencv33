using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;

class Program
{
    static void Main(string[] args)
    {
        // Загрузка изображения
        string imagePath = "D:\\4\\3.jpg"; // Путь к изображению
        Mat imgGray = CvInvoke.Imread(imagePath, ImreadModes.Grayscale); // Градации серого
        Mat imgColor = CvInvoke.Imread(imagePath, ImreadModes.Color);    // Цветное изображение

        // Применение адаптивной пороговой обработки
        Mat binaryImage = new Mat();
        CvInvoke.AdaptiveThreshold(
            imgGray, binaryImage, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, 11, 2);

        // Детектирование краёв методом Canny
        Mat edgesImage = new Mat();
        CvInvoke.Canny(binaryImage, edgesImage, 100, 200);

        // Поиск контуров
        using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
        {
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(edgesImage, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            // Рисование контуров на цветном изображении
            for (int i = 0; i < contours.Size; i++)
            {
                // Фильтрация слишком маленьких контуров (например, линии или шумы)
                Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);
                if (rect.Width > 10 && rect.Height > 10)
                {
                    // Рисование контура
                    CvInvoke.DrawContours(
                        imgColor, contours, i, new MCvScalar(0, 255, 0), 2); // Зелёный цвет, толщина 2 пикселя
                }
            }

            // Сохранение результата
            string resultPath = "D:\\4\\result_image_with_contours.jpg";
            CvInvoke.Imwrite(resultPath, imgColor); // Сохраняем результат в цвете

            Console.WriteLine($"Контуры успешно нарисованы и сохранены в '{resultPath}'");
        }
    }
}
