using System;
using System.Collections;
using System.Linq;
using System.Data;

namespace itcs3134a2pc
{
    class Program
    {
        static void Main(string[] args)
        {
            //creates string path to store text file
            const string file1 = "aorr9_hard.txt";
            //creates array of strings separated by line
            string[] lines = System.IO.File.ReadAllLines(file1).ToArray();
            string[] array = lines[0].Replace("[", "").Replace("]", "").Split(", ");
            //converts string array to int array
            int[] convertArray = array.Select(n => Convert.ToInt32(n)).ToArray();
            //final dimensions of grid (change depending on the amount of pixels)
            int height = 1024;
            int width = 1024;
            //2d array used to stack all pixel values
            int[,] stackIntensities = new int[height, width];
            //variable to count all rows passed
            int rowIndex = 0;
            //arraylist to collect rgb values
            ArrayList output = new ArrayList();
            output.Add("RGB");
            output.Add(1023.ToString());
            output.Add(1023.ToString());
            //for loop used to stack text file values into rows and columns
            for (int i = 0; i < height * width; i = i + width)
            {
                //since #rows = #columns row array can be either variable (height or width)
                int[] row = new int[width];
                int k = 0;
                for (int j = i; j < i + width; j++)
                {
                    //Console.Write(convertArray[j] + " ");
                    row[k] = convertArray[j];
                    //Console.Write(row[k] + " ");
                    k++;
                }
                //counts number of columns passed and adds it to stackIntensities array
                for (int l = 0; l < k; l++)
                {
                    stackIntensities[rowIndex, l] = row[l];
                    //Console.Write(stackIntensities[rowIndex, l] + " ");
                }
                //Console.WriteLine();
                rowIndex++;
            }
            //demosaicing by setting values based on rgb rows
            int r;
            int g;
            int b;
            //first rgb row used
            string colorRow = "BlueGreen";
            //Part C: reference green and red photodiodide lows, eventually moving row to blue and green
            bool lowerRightRed = false;
            bool lowerRightGreen = false;
            bool rgEnds = true;
            //for loop to sort rgb values
            for (int i = 0; i < height; i++)
            {
                lowerRightRed = false;
                lowerRightGreen = false;
                if (colorRow == "BlueGreen")
                {
                    string currentColor = "Blue";
                    for (int j = 0; j < width; j++)
                    {
                        if (currentColor == "Blue")
                        {
                            //after green photodiodide, the next is blue, repeating pattern until row is moved
                            //and if red and green do not make the lower ends

                            if (lowerRightGreen && !rgEnds)
                            {
                                double ul = stackIntensities[i - 1, j - 1] * 0.78;
                                double ur = stackIntensities[i - 1, j] * 0.9;
                                double ll = stackIntensities[i, j - 1] * 0.98;
                                double lr = stackIntensities[i, j] * 0.34;
                                r = Convert.ToInt32(ul);
                                g = Convert.ToInt32((ur + ll) / 2);
                                b = Convert.ToInt32(lr);
                                output.Add(r.ToString() + " " + g.ToString() + " " + b.ToString());
                                //lower right photodiodide is currently blue, prepares for transition to green (until row changes)
                                lowerRightGreen = false;
                            }
                            currentColor = "Green";

                        }
                        else
                        {
                            //after blue photodiodide, the next is green, repeating pattern until row is moved
                            //and if red and green do not make the lower ends
                            if (!lowerRightGreen && !rgEnds)
                            {
                                double ul = stackIntensities[i - 1, j - 1] * 0.9;
                                double ur = stackIntensities[i - 1, j] * 0.78;
                                double ll = stackIntensities[i, j - 1] * 0.34;
                                double lr = stackIntensities[i, j] * 0.98;
                                r = Convert.ToInt32(ur);
                                g = Convert.ToInt32((ul + lr) / 2);
                                b = Convert.ToInt32(ll);
                                output.Add(r.ToString() + " " + g.ToString() + " " + b.ToString());
                                //lower right photodiodide is currently green, prepares for transition to blue (until row changes)
                                lowerRightGreen = true;
                            }
                            currentColor = "Blue";
                        }
                        colorRow = "GreenRed";
                    }
                    //rows move down to green and red
                    rgEnds = true;
                }

                else
                {
                    string currentColor = "Green";
                    for (int j = 0; j < width; j++)
                    {
                        if (currentColor == "Green")
                        {
                            //after red photodiodide, the next is green, repeating pattern until row is moved
                            //and if red and green make the lower ends
                            if (lowerRightRed && rgEnds)
                            {
                                
                                double ul = stackIntensities[i - 1, j - 1] * 0.98;
                                double ur = stackIntensities[i - 1, j] * 0.34;
                                double ll = stackIntensities[i, j - 1] * 0.78;
                                double lr = stackIntensities[i, j] * 0.9;
                                r = Convert.ToInt32(ll);
                                g = Convert.ToInt32((ul + lr) / 2);
                                b = Convert.ToInt32(ur);
                                output.Add(r.ToString() + " " + g.ToString() + " " + b.ToString());
                                //lower right photodiodide is currently green, prepares for transition to red (until row changes)
                                lowerRightRed = false;
                                
                                
                            }
                            currentColor = "Red";
                        }
                        else
                        {
                            //after green photodiodide, the next is red, repeating pattern until row is moved
                            //and if red and green make the lower ends
                            //(ul = upper-left photodiodide, lr = lower-right photodiodide, etc)
                            if (!lowerRightRed && rgEnds)
                            {
                                double ul = stackIntensities[i - 1, j - 1] * 0.34;
                                double ur = stackIntensities[i - 1, j] * 0.98;
                                double ll = stackIntensities[i, j - 1] * 0.9;
                                double lr = stackIntensities[i, j] * 0.78;
                                r = Convert.ToInt32(lr);
                                g = Convert.ToInt32((ur + ll) / 2);
                                b = Convert.ToInt32(ul);
                                output.Add(r.ToString() + " " + g.ToString() + " " + b.ToString());
                                //lower right photodiodide is currently red, prepares for transition to green
                                lowerRightRed = true;
                            }
                            
                            currentColor = "Green";
                        }

                        colorRow = "BlueGreen";
                    }
                    //rows move down to blue and green
                    rgEnds = false;
                }
            }
            //converts output arraylist to string array and then creates text file
            string[] output1 = output.ToArray(typeof(string)) as string[];
            System.IO.File.WriteAllLines("output.txt", output1);
        }
    }
}
