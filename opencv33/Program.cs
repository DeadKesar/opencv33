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
        var scale = 10;
        // Загрузка изображения
        string imagePath = "D:\\4\\5.jpg"; // Путь к изображению
        Mat imgOriginal = CvInvoke.Imread(imagePath, ImreadModes.Color); // Оригинальное изображение

        // Сохранение исходного размера
        int originalWidth = imgOriginal.Width;
        int originalHeight = imgOriginal.Height;

        // Увеличение изображения в 10 раз
        Mat imgScaledUp = new Mat();
        CvInvoke.Resize(imgOriginal, imgScaledUp, new Size(originalWidth * scale, originalHeight * scale), 0, 0, Inter.Linear);

        // Конвертация в градации серого
        Mat imgGray = new Mat();
        CvInvoke.CvtColor(imgScaledUp, imgGray, ColorConversion.Bgr2Gray);

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

            // Рисование контуров на увеличенном изображении
            for (int i = 0; i < contours.Size; i++)
            {
                // Фильтрация слишком маленьких контуров (например, линии или шумы)
                Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);
                if (rect.Width > 10 && rect.Height > 10)
                {
                    // Рисование контура
                    CvInvoke.DrawContours(
                        imgScaledUp, contours, i, new MCvScalar(0, 255, 0), 2); // Зелёный цвет, толщина 2 пикселя
                }
            }

            // Уменьшение изображения обратно к исходному размеру
            Mat imgScaledDown = new Mat();
            CvInvoke.Resize(imgScaledUp, imgScaledDown, new Size(originalWidth, originalHeight), 0, 0, Inter.Area);

            // Сохранение результата
            string resultPath = "D:\\4\\result_image_with_resized_contours.jpg";
            CvInvoke.Imwrite(resultPath, imgScaledDown); // Сохраняем результат

            Console.WriteLine($"Контуры успешно обработаны, масштабированы и сохранены в '{resultPath}'");
        }
    }
}
