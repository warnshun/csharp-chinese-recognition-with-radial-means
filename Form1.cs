using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Chinese_recognition_with_radial_means
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = "1000"; //資料夾範圍--頭
            textBox2.Text = "1100"; //資料夾範圍--尾
            textBox3.Text = "0"; //檔案範圍--頭
            textBox4.Text = "100"; //檔案範圍--尾
            textBox5.Text = @"Z:\run_datas\bmp"; //來源資料夾位址
            textBox6.Text = @"Z:\run_datas\data"; //存檔資料夾位址
            textBox7.Text = "150"; //正規化寬度
            textBox8.Text = "5"; //圈數
            textBox9.Text = "4"; //角度分割數量
        }

        private void butCutting(object sender, EventArgs e)//裁邊按鈕
        {
            DateTime time_start = DateTime.Now;//計時開始 取得目前時間
            int i, j, headFolder, tailFolder, headPic, tailPic;
            string sourcePath, savePath, bmpSavePath, path;

            sourcePath = textBox5.Text;
            savePath = textBox6.Text + @"\newBmp";

            if (Directory.Exists(savePath))
                Directory.Delete(savePath, true);
            System.IO.Directory.CreateDirectory(savePath);

            headFolder = Convert.ToInt32(textBox1.Text);
            tailFolder = Convert.ToInt32(textBox2.Text);
            headPic = Convert.ToInt32(textBox3.Text);
            tailPic = Convert.ToInt32(textBox4.Text);
            for (i = headFolder; i <= tailFolder; i++)
            {
                System.IO.Directory.CreateDirectory(savePath + @"\" + i);
                for (j = headPic; j <= tailPic; j++)
                {
                    path = sourcePath + @"\" + i + @"\" + j + ".bmp";
                    bmpSavePath = savePath + @"\" + i + @"\" + j + ".bmp";
                    Cutting(path, bmpSavePath);
                }
            }

            System.Threading.Thread.Sleep(1000);
            DateTime time_end = DateTime.Now;//計時結束 取得目前時間
            string result = ((((TimeSpan)(time_end - time_start) ).TotalMilliseconds / 1000).ToString());
            label5.Text = "裁邊總花費時間" + result + "秒";

        }       

        public void Cutting(string path, string bmpSavePath)//裁邊
        {
            Bitmap bmp = new Bitmap(path);

            int[] data = FindEdge(bmp);
            int minX = data[0];
            int minY = data[1];
            int width = data[2];
            int height = data[3];

            Bitmap newBmp = bmp.Clone(new Rectangle(minX, minY, width, height), bmp.PixelFormat);
            newBmp.Save(bmpSavePath);
            bmp.Dispose();
        }

        public int[] FindEdge(Bitmap bmp)//尋找邊緣
        {            
            int width = bmp.Width, height = bmp.Height, x, y, minX = bmp.Width, minY = bmp.Height, maxX = 0, maxY = 0;
            int[] data = new int[4];

            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData BmData;
            IntPtr srcScan;

            BmData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            
            srcScan = BmData.Scan0;

            unsafe
            {
                byte* srcP = (byte*)srcScan;
                int srcOffset = BmData.Stride - width * 3;
                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++, srcP += 3)
                    {
                        if (*srcP == 0)
                        {
                            if (x < minX) minX = x;
                            if (y < minY) minY = y;
                            if (x > maxX) maxX = x;
                            if (y > maxY) maxY = y;
                        }
                    }
                    srcP += srcOffset;
                }
            }
            bmp.UnlockBits(BmData);

            return new int[] { minX, minY, (maxX - minX + 1), (maxY - minY + 1) };
        }

        private void butSource(object sender, EventArgs e)//來源瀏覽
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    textBox5.Text = fbd.SelectedPath;
                }
            }
        }

        private void butSave(object sender, EventArgs e)//存檔瀏覽
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    textBox6.Text = fbd.SelectedPath;
                }
            }
        }      

        private void butNormalization(object sender, EventArgs e)//正規按鈕
        {
            DateTime time_start = DateTime.Now;//計時開始 取得目前時間
            int i, j, headFolder, tailFolder, headPic, tailPic, width;
            string sourcePath, savePath, bmpSavePath, path;

            sourcePath = textBox6.Text + @"\newBmp";
            savePath = textBox6.Text + @"\Normalization";

            if (Directory.Exists(savePath))
                Directory.Delete(savePath, true);
            System.IO.Directory.CreateDirectory(savePath);

            headFolder = Convert.ToInt32(textBox1.Text);
            tailFolder = Convert.ToInt32(textBox2.Text);
            headPic = Convert.ToInt32(textBox3.Text);
            tailPic = Convert.ToInt32(textBox4.Text);
            width = Convert.ToInt32(textBox7.Text);
            for (i = headFolder; i <= tailFolder; i++)
            {
                System.IO.Directory.CreateDirectory(savePath + @"\" + i);
                for (j = headPic; j <= tailPic; j++)
                {
                    path = sourcePath + @"\" + i + @"\" + j + ".bmp";
                    bmpSavePath = savePath + @"\" + i + @"\" + j + ".bmp";
                    Normalization(path, bmpSavePath, width);
                }
            }

            System.Threading.Thread.Sleep(1000);
            DateTime time_end = DateTime.Now;//計時結束 取得目前時間
            string result = ((((TimeSpan)(time_end - time_start)).TotalMilliseconds / 1000).ToString());
            label5.Text = "正規化總花費時間" + result + "秒";
        }

        public void Normalization(string path, string bmpSavePath, int width)
        {
            Bitmap bmp = new Bitmap(path);

            int height;
            height = width; //長寬統一版本
            height = bmp.Height * width / bmp.Width;
            Bitmap newBmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(newBmp);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(Color.Transparent);
            g.DrawImage(bmp, new Rectangle(0, 0, width, height), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);

            newBmp = Thresholding(newBmp, 128);

            newBmp.Save(bmpSavePath);
            bmp.Dispose();
        }

        public Bitmap Thresholding(Bitmap bmp, int threshold)//二值化
        {
            int width = bmp.Width, height = bmp.Height, x, y;
            int[] data = new int[4];

            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData BmData;
            IntPtr srcScan;

            BmData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            srcScan = BmData.Scan0;

            unsafe
            {
                byte* srcP = (byte*)srcScan;
                int srcOffset = BmData.Stride - width * 3;
                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++, srcP += 3)
                    {
                        if (*srcP >= threshold)
                        {
                            *(srcP + 2) = *(srcP + 1) = *srcP = 255;
                        }
                        else
                        {
                            *(srcP + 2) = *(srcP + 1) = *srcP = 0;
                        }
                    }
                    srcP += srcOffset;
                }
            }
            bmp.UnlockBits(BmData);

            return bmp;
        }

        private void butContourk(object sender, EventArgs e)//輪廓按鈕
        {
            DateTime time_start = DateTime.Now;//計時開始 取得目前時間
            int i, j, headFolder, tailFolder, headPic, tailPic;
            string sourcePath, savePath, bmpSavePath, path;

            sourcePath = textBox6.Text + @"\newBmp";
            savePath = textBox6.Text + @"\Contour";
            /*
            if (Directory.Exists(savePath))
                Directory.Delete(savePath, true);
            System.IO.Directory.CreateDirectory(savePath);
            */
            headFolder = Convert.ToInt32(textBox1.Text);
            tailFolder = Convert.ToInt32(textBox2.Text);
            headPic = Convert.ToInt32(textBox3.Text);
            tailPic = Convert.ToInt32(textBox4.Text);
            for (i = headFolder; i <= tailFolder; i++)
            {
                System.IO.Directory.CreateDirectory(savePath + @"\" + i);
                for (j = headPic; j <= tailPic; j++)
                {
                    path = sourcePath + @"\" + i + @"\" + j + ".bmp";
                    bmpSavePath = savePath + @"\" + i + @"\" + j + ".bmp";
                    Contour(path, bmpSavePath);
                }
            }

            System.Threading.Thread.Sleep(1000);
            DateTime time_end = DateTime.Now;//計時結束 取得目前時間
            string result = ((((TimeSpan)(time_end - time_start)).TotalMilliseconds / 1000).ToString());
            label5.Text = "輪廓總花費時間" + result + "秒";
        }

        public void Contour(string path, string bmpSavePath)//輪廓
        {
            Bitmap bmp = new Bitmap(path);
            Bitmap newBmp = new Bitmap(bmp.Width, bmp.Height);
            int x, y;
            int[,] bmpArr = new int[bmp.Width, bmp.Height];
            int[,] newbmpArr = new int[newBmp.Width, newBmp.Height];

            Array.Clear(newbmpArr, 0, newbmpArr.Length);

            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData BmData;
            IntPtr srcScan;
            BmData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            srcScan = BmData.Scan0;
            unsafe
            {
                byte* srcP = (byte*)srcScan;
                int srcOffset = BmData.Stride - bmp.Width * 3;
                for (y = 0; y < bmp.Height; y++)
                {
                    for (x = 0; x < bmp.Width; x++, srcP += 3)
                    {
                        bmpArr[x, y] = *srcP;
                    }
                    srcP += srcOffset;
                }
            }
            bmp.UnlockBits(BmData);

            Graphics newbmpGrap = Graphics.FromImage(newBmp);
            newbmpGrap.FillRectangle(new SolidBrush(Color.White), 0, 0, newBmp.Width, newBmp.Height);
                        
            for (x = 0; x < bmp.Width; x++)//垂直掃瞄
            {
                y = 0;
                if (y == 0 && bmpArr[x, y] == 0)
                {
                    newbmpArr[x, y] = 1;
                }
                for (y = 1; y < bmp.Height; y++)
                {                    
                    if (bmpArr[x, y] != bmpArr[x, y - 1])
                    {
                        if (bmpArr[x, y] == 0)
                        {
                            newbmpArr[x, y] = 1;
                        }
                        else
                        {
                            newbmpArr[x, y - 1] = 1;
                        }
                    }
                }
                y = bmp.Height - 1;
                if (y == 0 && bmpArr[x, y] == 0)
                {
                    newbmpArr[x, y] = 1;
                }
            }//垂直結束

            for (y = 0; y < bmp.Height; y++)//水平掃瞄
            {
                x = 0;
                if (x == 0 && bmpArr[x, y] == 0)
                {
                    newbmpArr[x, y] = 1;
                }
                for (x = 1; x < bmp.Width; x++)
                {                    
                    if (bmpArr[x, y] != bmpArr[x - 1, y])
                    {
                        if (bmpArr[x, y] == 0)
                        {
                            newbmpArr[x, y] = 1;
                        }
                        else
                        {
                            newbmpArr[x - 1, y] = 1;
                        }
                    }
                }
                x = bmp.Width - 1;
                if (x == (bmp.Width - 1) && bmpArr[x, y] == 0)
                {
                    newbmpArr[x, y] = 1;
                }
            }//水平結束

            rect = new Rectangle(0, 0, newBmp.Width, newBmp.Height);
            BmData = newBmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            srcScan = BmData.Scan0;
            unsafe
            {
                byte* srcP = (byte*)srcScan;
                int srcOffset = BmData.Stride - newBmp.Width * 3;
                for (y = 0; y < newBmp.Height; y++)
                {
                    for (x = 0; x < newBmp.Width; x++, srcP += 3)
                    {
                        if(newbmpArr[x, y] == 1)
                        {
                            *(srcP + 2) = *(srcP + 1) = *srcP = 0;
                        }
                    }
                    srcP += srcOffset;
                }
            }
            newBmp.UnlockBits(BmData);

            newBmp.Save(bmpSavePath);
        }

        /*  不使用區域
        public Bitmap CopyBmp(Bitmap bmp, byte[] data, int x, int y, int width, int height)
        {
            int i, j, index = 0;

            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData BmData;
            IntPtr srcScan;

            BmData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            srcScan = BmData.Scan0;

            unsafe
            {
                byte* srcP = (byte*)srcScan;
                
                for (i = y; i < (y + height); i++) 
                {
                    for (j = x; j < (x + width); j++, index += 3)
                    {
                        srcP += CoordinateToPointer(j, i, bmp.Width);
                        *(srcP + 2) = *(srcP + 1) = *srcP = data[index];
                        srcP = (byte*)srcScan;
                    }
                }
            }
            bmp.UnlockBits(BmData);

            return bmp;
        }
        public int CoordinateToPointer(int x, int y, int bmpWidth)
        {
            return (x * 3) + y * 3 * bmpWidth;
        }
        public void PointerToCoordinate(int index, int bmpWidth)
        {
            int x, y, width = bmpWidth;
            x = (index % ((width + 1) * 3)) / 3;
            y = index / ((width + 1) * 3);
        }
        */

        private void butGetFeature(object sender, EventArgs e)
        {
            DateTime time_start = DateTime.Now;//計時開始 取得目前時間
            int i, j, headFolder, tailFolder, headPic, tailPic;
            string sourcePath, savePath, path;

            sourcePath = textBox6.Text + @"\Contour";
            savePath = textBox6.Text + @"\Feature";
            
            if (Directory.Exists(savePath))
                Directory.Delete(savePath, true);
            System.IO.Directory.CreateDirectory(savePath);

            headFolder = Convert.ToInt32(textBox1.Text);
            tailFolder = Convert.ToInt32(textBox2.Text);
            headPic = Convert.ToInt32(textBox3.Text);
            tailPic = Convert.ToInt32(textBox4.Text);
            for (i = headFolder; i <= tailFolder; i++)
            {
                StreamWriter featureTxt = new StreamWriter(savePath + @"\" + i + ".txt"); //創立txt(覆蓋原有)
                for (j = headPic; j <= tailPic; j++)
                {
                    path = sourcePath + @"\" + i + @"\" + j + ".bmp";
                    string str = GetFeature(path);
                    featureTxt.WriteLine(str);
                }                
                featureTxt.Close();
            }

            System.Threading.Thread.Sleep(1000);
            DateTime time_end = DateTime.Now;//計時結束 取得目前時間
            string result = ((((TimeSpan)(time_end - time_start)).TotalMilliseconds / 1000).ToString());
            label5.Text = "特徵總花費時間" + result + "秒";
        }
        public string GetFeature(string path)
        {
            int i;
            string str = null;

            Bitmap bmp = new Bitmap(path);

            Tuple<double, double, int> coc = GetCenterOfCircle(bmp);
            double centerOfCircleX = coc.Item1;
            double centerOfCircleY = coc.Item2;
            int totalPoint = coc.Item3;

            Tuple<double[], double[], double> wordData = GetWordData(bmp, centerOfCircleX, centerOfCircleY, totalPoint);
            double[] dis = wordData.Item1;
            double[] ang = wordData.Item2;
            double farthestDistance = wordData.Item3;

            double[] line = Ring(dis, farthestDistance);
            //for (int i = 0; i < line.Length; i++)
            //{
            //    if (i > 0) Console.Write(", ");
            //    Console.Write(line[i]);
            //}
            //Console.WriteLine();

            

            double[] feature = Angle(ang, dis, line);

            DrawRing(bmp, line, feature, centerOfCircleX, centerOfCircleY);

            for (i = 0; i < feature.Length; i++)
            {
                if (i > 0) str += ";";
                str += feature[i];
            }

            return str;
        }
        public Tuple<double, double, int> GetCenterOfCircle(Bitmap bmp)
        {
            int i, j, width = bmp.Width, height = bmp.Height, totalPoint = 0;
            double x = 0, y = 0;

            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData BmData;
            IntPtr srcScan;

            BmData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            srcScan = BmData.Scan0;

            unsafe
            {
                byte* srcP = (byte*)srcScan;
                int srcOffset = BmData.Stride - width * 3;
                for (i = 0; i < height; i++)
                {
                    for (j = 0; j < width; j++, srcP += 3)
                    {
                        if (*srcP == 0)
                        {
                            x += j;
                            y += i;
                            totalPoint++;
                        }
                    }
                    srcP += srcOffset;
                }
            }
            bmp.UnlockBits(BmData);

            x = x / totalPoint;
            y = y / totalPoint;            

            return new Tuple<double, double, int>(x, y, totalPoint);
        }

        public Tuple<double[], double[], double> GetWordData(Bitmap bmp, double x, double y, int point)
        {
            int i, j, count = 0;
            double farthestDistance = 0;

            double[] dis = new double[point];
            double[] ang = new double[point];

            for (i = 0; i < bmp.Width; i++)
            {
                for (j = 0; j < bmp.Height; j++)
                {
                    if (bmp.GetPixel(i, j).R == 0)
                    {

                        dis[count] = Math.Sqrt(((i - x) * (i - x)) + ((j - y) * (j - y)));
                        ang[count] = Math.Asin((double)(y - j) / dis[count]) * 180.0 / Math.PI;
                        if ((y - j) >= 0)
                        {
                            if ((i - x) < 0)
                            {
                                ang[count] = 180 - ang[count];
                            }
                        }
                        else
                        {
                            if ((i - x) < 0)
                            {
                                ang[count] = -180 - ang[count];
                            }
                            ang[count] += 360;
                        }

                        if (dis[count] > farthestDistance)
                        {
                            farthestDistance = dis[count];
                        }
                        count++;
                    }
                }
            }
            return new Tuple<double[], double[], double>(dis, ang, farthestDistance);
        }

        public double[] Ring( double[] dis, double farthestDistance) //距離資料, 最遠距離, 圓心, 座標
        {
            int i, index, nr = Convert.ToInt32(textBox8.Text);
            double spacing;
            double[] copyDis = new double[dis.Length];
            double[] line = new double[nr + 1]; //邊界

            line[0] = 0;
            line[nr] = farthestDistance;

            Array.Copy(dis, copyDis, dis.Length);
            Array.Sort(copyDis);

            spacing = dis.Length / (double)nr;

            for(i = 1; i < nr; i++)
            {
                index = (int)Math.Ceiling(spacing * i);
                line[i] = copyDis[index - 1];
            }

            return line;
        }

        public double AngleCorrection(double value)//角度修正
        {
            if (value < 0)
                return AngleCorrection(value += 360);
            else
                return (value % 360);
        }

        public double[] Angle(double[] ang, double[] dis, double[] line) //角度切割數, 圈數, 角度資料, 距離資料
        {
            int i, j, k, n, index, nr = Convert.ToInt32(textBox8.Text), na = Convert.ToInt32(textBox9.Text);
            double fz;
            double[] feature = new double[(nr * na)];
            double[] side = new double[na + 1];
            List<double> pir = new List<double>();

            for (i = 1; i <= na; i++)
                side[i] = (360 / na) * (i - 0.5);

            

            side[0] = 0;

            

            for (i = 0; i < nr; i++)
            {
                pir.Clear(); //清除環點數List

                for (j = 0; j < dis.Length; j++) //尋找符合環內點數
                {
                    if (dis[j] <= line[i + 1])
                    {
                        pir.Add(ang[j]);
                        dis[j] = line[nr] + 1;
                    }
                }

                fz = -side[na]; //取得歸零修正值

                for (j = 0; j < na; j++)
                    feature[(i * na) + j] = side[j % na];
            }
            return feature;
        }

        private void butAvgFeature(object sender, EventArgs e)
        {
            DateTime time_start = DateTime.Now;//計時開始 取得目前時間
            int i, j, k, numberOfData, headFolder, tailFolder, headPic, tailPic;
            string sourcePath, savePath, str;
            List<string> featureData = new List<string>();

            sourcePath = textBox6.Text + @"\Feature";
            savePath = textBox6.Text;
            numberOfData = Convert.ToInt32(textBox8.Text) * Convert.ToInt32(textBox9.Text);

            double[] avgFeature = new double[numberOfData];
            
            headFolder = Convert.ToInt32(textBox1.Text);
            tailFolder = Convert.ToInt32(textBox2.Text);
            headPic = Convert.ToInt32(textBox3.Text);
            tailPic = Convert.ToInt32(textBox4.Text);

            StreamWriter featureTxt = new StreamWriter(savePath + @"\avgFeature.txt"); //創立txt(覆蓋原有)
            for (i = headFolder; i <= tailFolder; i++)
            {
                featureData.Clear();
                Array.Clear(avgFeature, 0, avgFeature.Length);
                System.IO.StreamReader file = new System.IO.StreamReader(sourcePath + @"\" + i + ".txt");
                while ((str = file.ReadLine()) != null)
                {
                    featureData.Add(str);
                }
                for (j = headPic; j <= tailPic; j++)
                {
                    for (k = 0; k < avgFeature.Length; k++)
                    {
                        avgFeature[k] += double.Parse(featureData[j].Split(';')[k]);
                    }
                }
                for (k = 0; k < avgFeature.Length; k++)
                {
                    avgFeature[k] = avgFeature[k] / (double)(tailPic - headPic + 1);
                    if (k > 0) featureTxt.Write(";");
                    featureTxt.Write(avgFeature[k]);
                }
                featureTxt.WriteLine();
            }
                
            featureTxt.Close();

            System.Threading.Thread.Sleep(1000);
            DateTime time_end = DateTime.Now;//計時結束 取得目前時間
            string result = ((((TimeSpan)(time_end - time_start)).TotalMilliseconds / 1000).ToString());
            label5.Text = "資料總花費時間" + result + "秒";
        }

        private void butCompare(object sender, EventArgs e)
        {
            DateTime time_start = DateTime.Now;//計時開始 取得目前時間
            int i, j, k, l, numberOfData, headFolder, tailFolder, headPic, tailPic, index;
            double difference, minValue, count;
            string sourcePath, sourcePath2, savePath, str;
            List<string> avgFeatureData = new List<string>();
            List<string> featureData = new List<string>();
            List<double> differenceList = new List<double>();

            sourcePath = textBox6.Text;
            sourcePath2 = textBox6.Text + @"\Feature";
            savePath = textBox6.Text;

            numberOfData = Convert.ToInt32(textBox8.Text) * Convert.ToInt32(textBox9.Text);

            double[] feature = new double[numberOfData];

            headFolder = Convert.ToInt32(textBox1.Text);
            tailFolder = Convert.ToInt32(textBox2.Text);
            headPic = Convert.ToInt32(textBox3.Text);
            tailPic = Convert.ToInt32(textBox4.Text);
            
            System.IO.StreamReader file = new System.IO.StreamReader(sourcePath + @"\avgFeature.txt");
            while ((str = file.ReadLine()) != null)
            {
                avgFeatureData.Add(str);
            }

            StreamWriter accuracy = new StreamWriter(savePath + @"\Accuracy.txt"); //創立txt(覆蓋原有)
            for (i = headFolder; i <= tailFolder; i++)
            {
                count = 0;
                featureData.Clear();
                file = new System.IO.StreamReader(sourcePath2 + @"\" + i + ".txt");
                while ((str = file.ReadLine()) != null)
                {
                    featureData.Add(str);
                }
                for (j = headPic; j <= tailPic; j++)
                {
                    minValue = (360 * numberOfData);
                    index = 0;
                    for (k = 0; k < avgFeatureData.Count; k++)
                    {
                        
                        for (l = 0, difference = 0; l < numberOfData; l++)
                        {
                            difference += Math.Abs(double.Parse(avgFeatureData[k].Split(';')[l]) - double.Parse(featureData[j].Split(';')[l]));
                        }
                        if (difference < minValue)
                        {
                            minValue = difference;
                            index = k;
                        }
                    }
                    if ((index + headFolder) == i) count++;
                }
                accuracy.WriteLine((count / (double)(tailPic - headPic + 1) * 100));
            }
            accuracy.Close();

            System.Threading.Thread.Sleep(1000);
            DateTime time_end = DateTime.Now;//計時結束 取得目前時間
            string result = ((((TimeSpan)(time_end - time_start)).TotalMilliseconds / 1000).ToString());
            label5.Text = "比對總花費時間" + result + "秒";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            int i;
            double sum = 0;
            string sourcePath, savePath, str;
            List<double> accuracy = new List<double>();

            sourcePath = textBox6.Text + @"\Accuracy.txt";

            System.IO.StreamReader file = new System.IO.StreamReader(sourcePath);
            while ((str = file.ReadLine()) != null)
            {
                accuracy.Add(double.Parse(str));
            }

            for (i = 0; i < accuracy.Count; i++)
                sum += accuracy[i] / accuracy.Count;

            label5.Text = "整體的正確率: " + sum;
        }

        public void DrawRing(Bitmap bmp, double[] line, double[] side, double x, double y)
        {
            int i, na = Convert.ToInt32(textBox9.Text);
            double[] tarX = new double[na];
            double[] tarY = new double[na];

            Rectangle cloneRect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.PixelFormat format = bmp.PixelFormat;
            Bitmap newBmp = bmp.Clone(cloneRect, format);

            Graphics gra = Graphics.FromImage(newBmp);

            for (i = 1; i < line.Length; i++)
            {
                gra.DrawEllipse(new Pen(Color.Red, 1), (float)(x - line[i]), (float)(y - line[i]), (float)(line[i] * 2), (float)(line[i] * 2));
            }

            for(i = 0; i < na; i++)
            {
                tarX[i] = x + line[line.Length - 1] * Math.Cos(side[i + 1] * Math.PI / 180);
                tarY[i] = y - line[line.Length - 1] * Math.Sin(side[i + 1] * Math.PI / 180);
            }

            for (i = 0; i < na; i++)
            {
                gra.DrawLine(new Pen(Color.Blue, 1), (float)x, (float)y, (float)tarX[i], (float)tarY[i]); //畫邊界線
            }

            for (i = 1; i < side.Length; i++)
                Console.WriteLine(side[i]);

            newBmp.Save("1.bmp");
        }
    }
}
