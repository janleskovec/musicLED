using System;
using System.Windows.Forms;
using System.IO.Ports;
using System.Windows.Forms.DataVisualization.Charting;

using NAudio.Wave;
using MathNet.Numerics.IntegralTransforms;

namespace musicLED
{

    struct RGBcolor
    {
        public double r;
        public double g;
        public double b;
    };

    struct HSVcolor
    {
        public double h;
        public double s;
        public double v;
    };

    public partial class musicLED : Form
    {
        SerialPort port;

        int fftBufferLength = 600;
        int dataLength = 300;
        double[] leftSample = new double[602];
        double[] rightSample = new double[602];
        double[] fftInputLeft = new double[602];
        double[] fftInputRight = new double[602];
        double[] fftOutputLeft = new double[300];
        double[] fftOutputRight = new double[300];
        double[] dataForSerial = new double[6];
        double[] allData = new double[1200];
        double dataToHzMultiplier = 80;
        RGBcolor[] colors = new RGBcolor[602];
        RGBcolor lastColor;
        string LED_Mode;

        float redMultiplier = 1;
        float greenMultiplier = 1;
        float blueMultiplier = 1;
        float brightness = 1;

        float saturationMultiplier = 1.2f;

        float magnitude = 5;

        int counter = 0;

        IWaveIn waveIn;
        Timer timer;

        Form form;

        public musicLED()
        {
            Application.ApplicationExit += new EventHandler(OnApplicationExit);
            InitializeComponent();

            form = ActiveForm;

            //Find all serial ports and if there are any ports available, connect to the first one
            onRefresh(this, null);
            if (serialSelect.SelectedIndex > -1)
            {
                onConnect(this, null);
            }

            //Chart visualisation modes
            displayMode.Items.Add("All");
            displayMode.Items.Add("Average");
            displayMode.Items.Add("Peak");
            displayMode.Items.Add("Raw");
            displayMode.Items.Add("LED");
            displayMode.SelectedIndex = 4;

            //LED visualisation modes
            ledMode.Items.Add("Average");
            ledMode.Items.Add("Peak");
            ledMode.Items.Add("Average hue");
            ledMode.Items.Add("Average frequency hue");
            ledMode.Items.Add("Saturation enhancement - 1");
            ledMode.Items.Add("Saturation enhancement - 2");
            ledMode.Items.Add("DEBUG");
            ledMode.SelectedIndex = 2;
            LED_Mode = ledMode.SelectedItem.ToString();

            //Initialise loopback audio recording
            waveIn = new WasapiLoopbackCapture();
            waveIn.DataAvailable += WaveInDataAvailable;
            waveIn.StartRecording();

            //Start a timer to visualise data onto charts
            timer = new Timer();
            timer.Interval = 1;
            timer.Tick += new EventHandler(displayData);
            timer.Start();
        }

        //Callback for when data is available from the sound card
        private void WaveInDataAvailable(object sender, WaveInEventArgs e)
        {
            //Calculate the length of the fft buffer
            fftBufferLength = e.Buffer.Length / (waveIn.WaveFormat.BitsPerSample * waveIn.WaveFormat.Channels);
            dataLength = fftBufferLength / 2;

            double[] data = new double[e.Buffer.Length/32];
            for (int i = 0; i < data.Length; i += 1)
            {
                data[i] = BitConverter.ToSingle(e.Buffer, (i) * 4);
            }

            data.CopyTo(allData, 0);

            leftSample = new double[fftBufferLength + 2];
            rightSample = new double[fftBufferLength + 2];
            fftInputLeft = new double[fftBufferLength + 2];
            fftInputRight = new double[fftBufferLength + 2];

            int sampleSize = waveIn.WaveFormat.BitsPerSample / 2;

            //Read the buffer and copy the data to left and right channel arrays
            for (int i = 0; i < fftBufferLength; i += 2)
            {
                leftSample[i] = BitConverter.ToSingle(e.Buffer, i * 4);
                rightSample[i] = BitConverter.ToSingle(e.Buffer, (i + 1) * 4);
            }

            //Copy the data to a seperate array to prevent the data to be overwritten and preform Fourier transform
            leftSample.CopyTo(fftInputLeft, 0);
            rightSample.CopyTo(fftInputRight, 0);
            Fourier.ForwardReal(fftInputLeft, fftBufferLength);
            Fourier.ForwardReal(fftInputRight, fftBufferLength);
            for (int i = 0; i < fftInputLeft.Length; i++)
            {
                fftInputLeft[i] = Math.Abs(fftInputLeft[i]);
                fftInputRight[i] = Math.Abs(fftInputRight[i]);
            }
            fftOutputLeft = new double[fftBufferLength / 2];
            fftOutputRight = new double[fftBufferLength / 2];
            Array.Copy(fftInputLeft, 0, fftOutputLeft, 0, 300);
            Array.Copy(fftInputRight, 0, fftOutputRight, 0, 300);

            //Check which LED visualisation mode is selected and preform the selected calculations
            if (LED_Mode == "Average") {
                //Bands in Hz
                int[] ranges = { 0, 300, 1200, (int)(dataLength * dataToHzMultiplier) };

                double[] outputDataLeft = new double[ranges.Length - 1];
                double[] outputDataRight = new double[ranges.Length - 1];

                //Calculate the averages of the frequency bands
                for (int i = 0; i < ranges.Length - 1; i++)
                {
                    for (int n = (int)(ranges[i] / dataToHzMultiplier); n < (int)(ranges[i + 1] / dataToHzMultiplier); n++)
                    {
                        outputDataLeft[i] += fftOutputLeft[n];
                        outputDataRight[i] += fftOutputRight[n];
                    }
                    outputDataLeft[i] /= (ranges[i + 1] / dataToHzMultiplier) - (ranges[i] / dataToHzMultiplier);
                    outputDataRight[i] /= (ranges[i + 1] / dataToHzMultiplier) - (ranges[i] / dataToHzMultiplier);
                }

                //Call the function that sends the data to a microcontroller
                SendData(new double[]{
                    outputDataLeft[2],
                    outputDataLeft[1],
                    outputDataLeft[0],
                    outputDataRight[2],
                    outputDataRight[1],
                    outputDataRight[0]
                });
            }
            else if (LED_Mode == "Peak")
            {
                //Bands in Hz
                int[] ranges = { 0, 300, 1200, (int)(dataLength * dataToHzMultiplier) };

                double[] outputDataLeft = new double[ranges.Length - 1];
                double[] outputDataRight = new double[ranges.Length - 1];

                //Get the peak value of every frequency band
                for (int i = 0; i < ranges.Length - 1; i++)
                {
                    outputDataLeft[i] = 0;
                    outputDataRight[i] = 0;
                    for (int n = (int)(ranges[i] / dataToHzMultiplier); n < (int)(ranges[i + 1] / dataToHzMultiplier); n++)
                    {
                        if (outputDataLeft[i] < fftOutputLeft[n])
                        {
                            outputDataLeft[i] = fftOutputLeft[n];
                        }

                        if (outputDataRight[i] < fftOutputRight[n])
                        {
                            outputDataRight[i] = fftOutputRight[n];
                        }
                    }
                }

                //Ensure the values are between 0 and 1
                for (int i = 0; i < outputDataLeft.Length; i++)
                {
                    outputDataLeft[i] /= outputDataLeft[i] + 1;
                    outputDataRight[i] /= outputDataRight[i] + 1;
                }

                //Call the function that sends the data to a microcontroller
                SendData(new double[]{
                    outputDataLeft[2],
                    outputDataLeft[1],
                    outputDataLeft[0],
                    outputDataRight[2],
                    outputDataRight[1],
                    outputDataRight[0]
                });
            }
            else if (LED_Mode == "Average hue")
            {
                //Convert the fft output to RGB based on the hsv color space
                RGBcolor outputLeft = new RGBcolor();
                RGBcolor outputRight = new RGBcolor();
                for (int i = 0; i < dataLength; i++)
                {
                    outputLeft.r += colors[i].r * fftOutputLeft[i];
                    outputLeft.g += colors[i].g * fftOutputLeft[i];
                    outputLeft.b += colors[i].b * fftOutputLeft[i];

                    outputRight.r += colors[i].r * fftOutputRight[i];
                    outputRight.g += colors[i].g * fftOutputRight[i];
                    outputRight.b += colors[i].b * fftOutputRight[i];
                }

                outputLeft.r /= dataLength;
                outputLeft.g /= dataLength;
                outputLeft.b /= dataLength;
                outputRight.r /= dataLength;
                outputRight.g /= dataLength;
                outputRight.b /= dataLength;

                //Proccess the colors a bit
                float x = 400;

                outputLeft.r *= x;
                outputLeft.g *= x;
                outputLeft.b *= x;
                outputRight.r *= x;
                outputRight.g *= x;
                outputRight.b *= x;

                outputLeft.r = Math.Pow(outputLeft.r, magnitude);
                outputLeft.g = Math.Pow(outputLeft.g, magnitude);
                outputLeft.b = Math.Pow(outputLeft.b, magnitude);
                outputRight.r = Math.Pow(outputRight.r, magnitude);
                outputRight.g = Math.Pow(outputRight.g, magnitude);
                outputRight.b = Math.Pow(outputRight.b, magnitude);


                outputLeft.r /= x;
                outputLeft.g /= x;
                outputLeft.b /= x;
                outputRight.r /= x;
                outputRight.g /= x;
                outputRight.b /= x;

                //I'm trying to figure out a way to have the brightness adapt to the volume so there would be more detail. Now That I think about it it does sound like HDR haha...
                /*double maxLeft = Math.Max(outputLeft.r,Math.Max(outputLeft.g, outputLeft.b));
                outputLeft.r /= maxLeft;
                outputLeft.g /= maxLeft;
                outputLeft.b /= maxLeft;
                double maxRight = Math.Max(outputRight.r, Math.Max(outputRight.g, outputRight.b));
                outputRight.r /= maxRight;
                outputRight.g /= maxRight;
                outputRight.b /= maxRight;*/


                //Ensure the RGB chanels are between 0 and 1
                outputLeft.r /= outputLeft.r + 1;
                outputLeft.g /= outputLeft.g + 1;
                outputLeft.b /= outputLeft.b + 1;
                outputRight.r /= outputRight.r + 1;
                outputRight.g /= outputRight.g + 1;
                outputRight.b /= outputRight.b + 1;

                //Call the function that sends the data to a microcontroller
                SendData(new double[]{
                    outputLeft.r,
                    outputLeft.g,
                    outputLeft.b,
                    outputRight.r,
                    outputRight.g,
                    outputRight.b
                });

            }
            else if (LED_Mode == "Average frequency hue")
            {
                //Calculate average frequency and amplitude
                RGBcolor outputLeft = new RGBcolor();
                RGBcolor outputRight = new RGBcolor();
                double totalAmplitudeLeft = 0;
                double totalAmplitudeRight = 0;
                double averagefrequencyLeft = 0;
                double averagefrequencyRight = 0;
                double averageAmplitudeLeft = 0;
                double averageAmplitudeRight = 0;
                for (int i = 0; i < dataLength; i++)
                {

                    totalAmplitudeLeft += fftOutputLeft[i];
                    totalAmplitudeRight += fftOutputRight[i];

                    averagefrequencyLeft += fftOutputLeft[i] * i;
                    averagefrequencyRight += fftOutputRight[i] * i;
                }

                averagefrequencyLeft /= totalAmplitudeLeft;
                averagefrequencyRight /= totalAmplitudeRight;
                averageAmplitudeLeft = totalAmplitudeLeft / dataLength;
                averageAmplitudeRight = totalAmplitudeRight / dataLength;

                if (!Double.IsNaN(averagefrequencyLeft))
                {
                    outputLeft = colors[(int)averagefrequencyLeft];
                    outputLeft.r *= averageAmplitudeLeft;
                    outputLeft.g *= averageAmplitudeLeft;
                    outputLeft.b *= averageAmplitudeLeft;
                }
                else
                {
                    outputLeft = new RGBcolor();
                }

                if (!Double.IsNaN(averagefrequencyRight))
                {
                    outputRight = colors[(int)averagefrequencyRight];
                    outputRight.r *= averageAmplitudeRight;
                    outputRight.g *= averageAmplitudeRight;
                    outputRight.b *= averageAmplitudeRight;
                }
                else
                {
                    outputRight = new RGBcolor();
                }

                int x = 20;

                outputLeft.r *= x;
                outputLeft.g *= x;
                outputLeft.b *= x;

                outputRight.r *= x;
                outputRight.g *= x;
                outputRight.b *= x;

                //Call the function that sends the data to a microcontroller
                SendData(new double[]{
                    outputLeft.r,
                    outputLeft.g,
                    outputLeft.b,
                    outputRight.r,
                    outputRight.g,
                    outputRight.b
                });
            }
            else if (LED_Mode == "Saturation enhancement - 1")
            {
                RGBcolor outputLeft = new RGBcolor();
                RGBcolor outputRight = new RGBcolor();
                for (int i = 0; i < dataLength; i++)
                {
                    outputLeft.r += colors[i].r * fftOutputLeft[i];
                    outputLeft.g += colors[i].g * fftOutputLeft[i];
                    outputLeft.b += colors[i].b * fftOutputLeft[i];

                    outputRight.r += colors[i].r * fftOutputRight[i];
                    outputRight.g += colors[i].g * fftOutputRight[i];
                    outputRight.b += colors[i].b * fftOutputRight[i];
                }

                outputLeft.r /= dataLength;
                outputLeft.g /= dataLength;
                outputLeft.b /= dataLength;
                outputRight.r /= dataLength;
                outputRight.g /= dataLength;
                outputRight.b /= dataLength;

                float x = 10;

                outputLeft.r *= x;
                outputLeft.g *= x;
                outputLeft.b *= x;
                outputRight.r *= x;
                outputRight.g *= x;
                outputRight.b *= x;

                HSVcolor hsvLeft = RGBtoHSV(outputLeft);
                hsvLeft.s *= saturationMultiplier;
                outputLeft = HSVToRGB(hsvLeft);

                HSVcolor hsvRight = RGBtoHSV(outputRight);
                hsvRight.s *= saturationMultiplier;
                outputRight = HSVToRGB(hsvRight);

                //Call the function that sends the data to a microcontroller
                SendData(new double[]{
                    outputLeft.r,
                    outputLeft.g,
                    outputLeft.b,
                    outputRight.r,
                    outputRight.g,
                    outputRight.b
                });
            }
            else if (LED_Mode == "Saturation enhancement - 2")
            {
                //Calculate average frequency and amplitude
                RGBcolor outputLeft = new RGBcolor();
                RGBcolor outputRight = new RGBcolor();
                double totalAmplitudeLeft = 0;
                double totalAmplitudeRight = 0;
                double averagefrequencyLeft = 0;
                double averagefrequencyRight = 0;
                double averageAmplitudeLeft = 0;
                double averageAmplitudeRight = 0;
                for (int i = 0; i < dataLength; i++)
                {

                    totalAmplitudeLeft += fftOutputLeft[i];
                    totalAmplitudeRight += fftOutputRight[i];

                    averagefrequencyLeft += fftOutputLeft[i] * i;
                    averagefrequencyRight += fftOutputRight[i] * i;
                }

                averagefrequencyLeft /= totalAmplitudeLeft;
                averagefrequencyRight /= totalAmplitudeRight;
                averageAmplitudeLeft = totalAmplitudeLeft / dataLength;
                averageAmplitudeRight = totalAmplitudeRight / dataLength;

                if (!Double.IsNaN(averagefrequencyLeft))
                {
                    outputLeft = colors[(int)averagefrequencyLeft];
                    outputLeft.r *= averageAmplitudeLeft;
                    outputLeft.g *= averageAmplitudeLeft;
                    outputLeft.b *= averageAmplitudeLeft;
                }
                else
                {
                    outputLeft = new RGBcolor();
                }

                if (!Double.IsNaN(averagefrequencyRight))
                {
                    outputRight = colors[(int)averagefrequencyRight];
                    outputRight.r *= averageAmplitudeRight;
                    outputRight.g *= averageAmplitudeRight;
                    outputRight.b *= averageAmplitudeRight;
                }
                else
                {
                    outputRight = new RGBcolor();
                }

                int x = 10;

                outputLeft.r *= x;
                outputLeft.g *= x;
                outputLeft.b *= x;

                outputRight.r *= x;
                outputRight.g *= x;
                outputRight.b *= x;

                HSVcolor hsvLeft = RGBtoHSV(outputLeft);
                hsvLeft.s *= saturationMultiplier;
                outputLeft = HSVToRGB(hsvLeft);

                HSVcolor hsvRight = RGBtoHSV(outputRight);
                hsvRight.s *= saturationMultiplier;
                outputRight = HSVToRGB(hsvRight);

                //Call the function that sends the data to a microcontroller
                SendData(new double[]{
                    outputLeft.r,
                    outputLeft.g,
                    outputLeft.b,
                    outputRight.r,
                    outputRight.g,
                    outputRight.b
                });
            }
            else if (LED_Mode == "DEBUG")
            {
                RGBcolor outputLeft = new RGBcolor();
                RGBcolor outputRight = new RGBcolor();

                HSVcolor color = new HSVcolor();
                color.h = counter / 100.0f;
                color.s = 0.5f;
                color.v = 1;
                outputLeft = HSVToRGB(color);
                outputRight = HSVToRGB(color);

                HSVcolor hsvLeft = RGBtoHSV(outputLeft);
                hsvLeft.s = 1;
                outputLeft = HSVToRGB(hsvLeft);

                //HSVcolor hsvRight = RGBtoHSV(outputRight);
                //hsvRight.s = 1;
                //outputRight = HSVToRGB(hsvRight);

                //Call the function that sends the data to a microcontroller
                SendData(new double[]{
                    outputLeft.r,
                    outputLeft.g,
                    outputLeft.b,
                    outputRight.r,
                    outputRight.g,
                    outputRight.b
                });

                counter %= 100;
                counter++;
            }
        }

        //Timer callback for displaying the fft data on a chart
        private void displayData(object sender, EventArgs e)
        {
            //Correctly set the connect/disconnect button text
            if (port != null && port.IsOpen)
            {
                connectButton.Text = "Disconnect";
            }
            else
            {
                connectButton.Text = "Connect";
            }

            //Visualise the data onto a chart if the checkbox is checked
            if (displayCheckBox.Checked)
            {
                chartFft.Visible = true;
                chartFft.Series[0].Points.Clear();
                //Full data
                if (displayMode.SelectedItem.ToString() == "All")
                {
                    chartFft.Series[0].ChartType = SeriesChartType.Area;
                    for (int i = 0; i < fftOutputLeft.Length; i++)
                    {
                        chartFft.ChartAreas[0].AxisX.Title = "Hz";
                        chartFft.ChartAreas[0].AxisY.Maximum = 4;
                        chartFft.ChartAreas[0].AxisY.Minimum = 0;
                        chartFft.Series[0].Points.AddXY((i + 1) * dataToHzMultiplier, fftOutputLeft[i]);
                    }
                }

                //Averages from specific bands
                else if (displayMode.SelectedItem.ToString() == "Average")
                {
                    chartFft.Series[0].ChartType = SeriesChartType.Column;
                    //Bands in Hz: 0 100 300 600 1200 2400 4800 other
                    int[] ranges = { 0, 100, 300, 600, 1200, 2400, 4800, (int)(dataLength * dataToHzMultiplier) };
                    double[] displayDataLeft = new double[ranges.Length - 1];

                    for (int i = 0; i < ranges.Length - 1; i++)
                    {
                        for (int n = (int)(ranges[i] / dataToHzMultiplier); n < (int)(ranges[i + 1] / dataToHzMultiplier); n++)
                        {
                            displayDataLeft[i] += fftOutputLeft[n];
                        }
                        displayDataLeft[i] /= (ranges[i + 1] / dataToHzMultiplier) - (ranges[i] / dataToHzMultiplier);
                    }

                    for (int i = 0; i < displayDataLeft.Length; i++)
                    {
                        chartFft.ChartAreas[0].AxisX.Title = "Bands";
                        chartFft.ChartAreas[0].AxisY.Maximum = 1;
                        chartFft.ChartAreas[0].AxisY.Minimum = 0;
                        chartFft.Series[0].Points.AddXY(i + 1, displayDataLeft[i]);
                    }
                }

                //Peaks from specific bands
                else if (displayMode.SelectedItem.ToString() == "Peak")
                {
                    chartFft.Series[0].ChartType = SeriesChartType.Column;
                    //Bands in Hz: 0 100 300 600 1200 2400 4800 other
                    int[] ranges = { 0, 100, 300, 600, 1200, 2400, 4800, (int)(dataLength * dataToHzMultiplier) };
                    double[] displayDataLeft = new double[ranges.Length - 1];

                    for (int i = 0; i < ranges.Length - 1; i++)
                    {
                        displayDataLeft[i] = 0;
                        for (int n = (int)(ranges[i] / dataToHzMultiplier); n < (int)(ranges[i + 1] / dataToHzMultiplier); n++)
                        {
                            if (displayDataLeft[i] < fftOutputLeft[n])
                            {
                                displayDataLeft[i] = fftOutputLeft[n];
                            }
                        }
                    }

                    for (int i = 0; i < displayDataLeft.Length; i++)
                    {
                        displayDataLeft[i] /= displayDataLeft[i] + 1;
                        chartFft.ChartAreas[0].AxisX.Title = "Bands";
                        chartFft.ChartAreas[0].AxisY.Maximum = 1;
                        chartFft.ChartAreas[0].AxisY.Minimum = 0;
                        chartFft.Series[0].Points.AddXY(i + 1, displayDataLeft[i]);
                    }
                }

                //Raw data
                else if (displayMode.SelectedItem.ToString() == "Raw")
                {
                    chartFft.Series[0].ChartType = SeriesChartType.Line;
                    for (int i = 0; i < allData.Length; i++)
                    {
                        chartFft.ChartAreas[0].AxisX.Title = "Time";
                        chartFft.ChartAreas[0].AxisY.Maximum = 1;
                        chartFft.ChartAreas[0].AxisY.Minimum = -1;
                        chartFft.Series[0].Points.AddXY(i + 1, allData[i]);
                    }
                }

                //LED data
                else if (displayMode.SelectedItem.ToString() == "LED")
                {
                    chartFft.Series[0].ChartType = SeriesChartType.Bar;
                    chartFft.ChartAreas[0].AxisX.Title = "RGB";
                    chartFft.ChartAreas[0].AxisY.Maximum = 1;
                    chartFft.ChartAreas[0].AxisY.Minimum = 0;
                    chartFft.Series[0].Points.AddXY("R", lastColor.r);
                    chartFft.Series[0].Points.AddXY("G", lastColor.g);
                    chartFft.Series[0].Points.AddXY("B", lastColor.b);
                }
            }
        }

        //Function that packages the RGB data and sends it to the microcontroller via serial
        private void SendData(double[] data)
        {

            lastColor.r = (data[0] + data[3]) / 2;
            lastColor.g = (data[1] + data[4]) / 2;
            lastColor.b = (data[2] + data[5]) / 2;

            /*lastColor.r = data[0];
            lastColor.g = data[1];
            lastColor.b = data[2];*/

            for (int i = 0; i < 6; i++)
            {
                //Cutoff
                if (data[i] > 1)
                {
                    data[i] = 1;
                }
                //Smoothing
                if (smoothingCheckBox.Checked)
                {
                    dataForSerial[i] = Lerp(dataForSerial[i], data[i], 0.8f);
                }
                else
                {
                    dataForSerial[i] = data[i];
                }
                dataForSerial[i] *= brightness;
            }

            dataForSerial[0] *= redMultiplier;
            dataForSerial[3] *= redMultiplier;

            dataForSerial[1] *= greenMultiplier;
            dataForSerial[4] *= greenMultiplier;

            dataForSerial[2] *= blueMultiplier;
            dataForSerial[5] *= blueMultiplier;



            //Convert the data to bytes and send it via serial
            byte[] output = new byte[6];

            //LEFT
            //RED
            output[0] = (byte)(int)(dataForSerial[0] * 255);
            //GREEN
            output[1] = (byte)(int)(dataForSerial[1] * 255);
            //BLUE
            output[2] = (byte)(int)(dataForSerial[2] * 255);

            //RIGHT
            //RED
            output[3] = (byte)(int)(dataForSerial[3] * 255);
            //GREEN
            output[4] = (byte)(int)(dataForSerial[4] * 255);
            //BLUE
            output[5] = (byte)(int)(dataForSerial[5] * 255);

            if (port != null && port.IsOpen)
            {
                try
                {
                    port.Write(output, 0, 6);
                }
                catch { }
            }
        }

        //Linearly interpolates between two values
        private double Lerp(double value1, double value2, double amount)
        {
            return (value1 * (1 - amount)) + (value2 * amount);
        }

        //Converts hue (0-1) to an RGB value
        private RGBcolor HueToRGB(double hue)
        {
            //get values between 0 and 1
            hue = hue % 1;

            RGBcolor output = new RGBcolor();
            double x = (hue * 6) % 1;
            if (hue < (1f / 6f))
            {
                output.r = 1f;
                output.g = x;
                output.b = 0f;
            }
            else if (hue < (2f / 6f))
            {
                output.r = 1f - x;
                output.g = 1f;
                output.b = 0f;
            }
            else if (hue < (3f / 6f))
            {
                output.r = 0f;
                output.g = 1f;
                output.b = x;
            }
            else if (hue < (4f / 6f))
            {
                output.r = 0f;
                output.g = 1f - x;
                output.b = 1f;
            }
            else if (hue < (5f / 6f))
            {
                output.r = x;
                output.g = 0f;
                output.b = 1f;
            }
            else
            {
                output.r = 1f;
                output.g = 0f;
                output.b = 1f - x;
            }
            return output;
        }

        //Converts hsv to rgb
        private RGBcolor HSVToRGB(HSVcolor color)
        {
            RGBcolor output;
            output.r = 0;
            output.g = 0;
            output.b = 0;

            if (color.s < 1)
            {
                color.s = 1;
            }

            output = HueToRGB(color.h);

            output.r = Lerp(output.r, 1, 1 - color.s);
            output.g = Lerp(output.g, 1, 1 - color.s);
            output.b = Lerp(output.b, 1, 1 - color.s);

            output.r *= color.v;
            output.g *= color.v;
            output.b *= color.v;

            return output;
        }

        //Converts rgb to hsv
        private HSVcolor RGBtoHSV(RGBcolor color)
        {
            HSVcolor output;
            output.h = 0;
            output.s = 0;
            output.v = 0;

            double min, max, delta;

            min = Math.Min(color.r, color.g);
            min = Math.Min(min, color.b);

            max = Math.Max(color.r, color.g);
            max = Math.Max(max, color.b);

            output.v = max;
            delta = max - min;
            output.s = (delta / max);

            if ( color.r >= max )
                output.h = (color.g - color.b ) / delta;
            else if (color.g >= max )
                output.h = 2.0 + (color.b - color.r ) / delta;
            else
                output.h = 4.0 + (color.r - color.g ) / delta;

            output.h /= 6;
            if (output.h < 0)
                output.h += 1;

            return output;
        }

        //Stop recording and close the serial port
        private void OnApplicationExit(object sender, EventArgs e)
        {
            waveIn.StopRecording();
            port.Close();
        }

        //Refreshes the available serial ports
        private void onRefresh(object sender, EventArgs e)
        {
            serialSelect.Items.Clear();
            serialSelect.Items.AddRange(SerialPort.GetPortNames());
            if (serialSelect.Items.Count > 0)
            {
                serialSelect.SelectedIndex = 0;
            }
        }

        //Open the selected serial port
        private void onConnect(object sender, EventArgs e)
        {
            if (port != null && port.IsOpen)
            {
                port.Close();
            }
            else if (serialSelect.SelectedItem != null)
            {
                try
                {
                    port = new SerialPort(serialSelect.SelectedItem.ToString(), 9600, Parity.None, 8, StopBits.One);
                    port.Open();
                }
                catch
                {
                    MessageBox.Show("Port busy or is allredy connected");
                }

            }
        }

        //The user has changed the hue shift slider, recalculate the colors
        private void ChangeColorHue(bool useLogScale, float hueShift = 0)
        {
            for (int i = 0; i < dataLength; i++)
            {
                double hue = (float)i / dataLength;
                hue += hueShift;
                //I used this to increase the "colorfulness"
                //hue *= 4;
                hue %= 1;
                hue = (Math.Log10(hue + 0.01f) + 2) / 2;
                hue %= 1;
                colors[i] = HueToRGB(hue);
            }
        }

        //The user has changed the chart visibility
        private void onDisplayChanged(object sender, EventArgs e)
        {
            if (displayCheckBox.Checked)
            {
                chartFft.Visible = true;
                ActiveForm.Size = new System.Drawing.Size(640, 560);
            }
            else
            {
                chartFft.Visible = false;
                ActiveForm.Size = new System.Drawing.Size(640, 260);
            }
        }

        //LED visualisation mode has changed
        private void onLEDModeChanged(object sender, EventArgs e)
        {
            LED_Mode = ledMode.SelectedItem.ToString();
            ChangeColorHue(true, 0);
            if (LED_Mode == "Average hue")
            {
                ChangeColorHue(true, 0);
            }else if (LED_Mode == "Average frequency hue")
            {
                ChangeColorHue(true, 0);
            }
        }

        //The user has changed the red slider value
        private void OnRedSliderChanged(object sender, EventArgs e)
        {
            redMultiplier = (float)redSlider.Value / redSlider.Maximum;
        }

        //The user has changed the green slider value
        private void OnGreenSliderChanged(object sender, EventArgs e)
        {
            greenMultiplier = (float)greenSlider.Value / greenSlider.Maximum;
        }

        //The user has changed the blue slider value
        private void OnBlueSliderChanged(object sender, EventArgs e)
        {
            blueMultiplier = (float)blueSlider.Value / blueSlider.Maximum;
        }

        //The user has changed the brightness slider value
        private void OnBrightnessSliderChanged(object sender, EventArgs e)
        {
            brightness = ((float)brightnessSlider.Value / brightnessSlider.Maximum) * 2;
        }
    }
}