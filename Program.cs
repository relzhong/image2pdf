using CmdLine;
using ImageMagick;
using System;
using System.IO;

namespace image2pdf
{
    [CommandLineArguments(Program = "image2pdf", Title = "image to pdf convertor", Description = "")]
    class SimpleCopyArguments
    {
        [CommandLineParameter(Command = "?", Default = false, Description = "Show Help", Name = "Help", IsHelp = true)]
        public bool Help { get; set; }

        [CommandLineParameter(Name = "source", ParameterIndex = 1, Required = true, Description = "Specifies the image to be convert.")]
        public string Source { get; set; }

        [CommandLineParameter(Name = "destination", ParameterIndex = 2, Required = true, Description = "Specifies the directory and/or filename for the new file(s).")]
        public string Destination { get; set; }

        [CommandLineParameter(Name = "dpi", ParameterIndex = 3, Default = 0, Description = "Specifies the dpi of source image.")]
        public int Dpi { get; set; }

        [CommandLineParameter(Command = "L", Description = "Indicates a landscape image")]
        public bool Landscape { get; set; }

    }
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SimpleCopyArguments arguments = CommandLine.Parse<SimpleCopyArguments>();
                if (!File.Exists(arguments.Source) && !Directory.Exists(arguments.Destination))
                {
                    throw new Exception("path error");
                }
                using (MagickImage image = new MagickImage(arguments.Source))
                {
                    // default A4 portrait
                    int printWidth = 595;
                    int printHeight = 842;
                    int pageNum = 1;
                    int width = image.Width;
                    int height = image.Height;
                    Console.WriteLine("width: " + pageNum);
                    Console.WriteLine("height: " + pageNum);
                    pageNum = (int)Math.Round((1.0 * width / height) / (1.0 * printWidth / printHeight), MidpointRounding.AwayFromZero);
                    if (arguments.Landscape)
                    {
                        pageNum = (int)Math.Round((1.0 * height / width) / (1.0 * printWidth / printHeight), MidpointRounding.AwayFromZero);
                    }
                    Console.WriteLine("pageNum: " + pageNum);
                    if (arguments.Dpi > 0)
                    {
                       image.Density = new Density(arguments.Dpi);
                    }
                    Console.WriteLine("Density: " + image.Density);
                    // Create pdf file with a single page
                    using ( MagickImageCollection collection = new MagickImageCollection())
                    {
                        for (int i = 0; i < pageNum; i++)
                        {
                            IMagickImage page;
                            if (arguments.Landscape)
                            {
                                page = image.Clone(0, i * (int)Math.Floor(1.0 * printWidth * width / printHeight), width, (int)Math.Floor(1.0 * printWidth * width / printHeight));
                                page.Rotate(90);
                            } else
                            {
                                page = image.Clone(0, i * (int)Math.Floor(1.0 * printHeight * width / printWidth), width, (int)Math.Floor(1.0 * printHeight * width / printWidth));
                            }
                            // Add page
                            collection.Add(new MagickImage(page));
                        }
                        // Create pdf file with two pages
                        collection.Write(arguments.Destination);
                    }
                }
            }
            catch (CommandLineException exception)
            {
                Console.WriteLine(exception.ArgumentHelp.Message);
                Console.WriteLine(exception.ArgumentHelp.GetHelpText(Console.BufferWidth));
            }
            
        }
    }
}
 