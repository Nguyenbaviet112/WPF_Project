using Aspose.Cells;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bai01
{


    //}
    /// <summary>
    /// Interaction logic for DashboardWindow.xaml
    /// </summary>
    public partial class DashboardWindow : Window
    {
        public delegate void DeathHandler();
        public event DeathHandler Dying;
        public DashboardWindow()
        {
            InitializeComponent();
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (Dying != null)
            {
                Dying.Invoke(); //Dying?.Invoke();
            }    
            
            this.Close();
        }

        private void importButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = new OpenFileDialog();
            if (screen.ShowDialog() == true)
            {

                var filename = screen.FileName;
                var info = new FileInfo(filename);

                var excelFile = new Workbook(screen.FileName);
                var tabs = excelFile.Worksheets;

                var db = new MystoreEntities();

                foreach (var tab in tabs)
                {
                    var row = 3;
                    var cell = tab.Cells[$"C{3}"];

                    var category = new Category()
                    {
                        Name = tab.Name
                    };
                    db.Categories.Add(category);
                    db.SaveChanges();

                    while (cell.Value != null)
                    {
                        var imagesFolder = info.Directory + "\\images";
                        var sku = tab.Cells[$"C{row}"].StringValue;
                        var name = tab.Cells[$"D{row}"].StringValue;
                        var price = tab.Cells[$"E{row}"].IntValue;
                        var quantity = tab.Cells[$"F{row}"].IntValue;
                        var description = tab.Cells[$"G{row}"].StringValue;
                        var image = tab.Cells[$"H{row}"].StringValue;
                        imagesFolder = imagesFolder + "\\" + image;


                        var product = new Product()
                        {
                            SKU = sku,
                            Price = price,
                            Quantity = quantity,
                            Description = description,
                            Name = name,
                            CatId = category.Id,
                            Image = image
                        };
                        category.Products.Add(product);
                        db.SaveChanges();


                        if (File.Exists(imagesFolder))
                        {

                            var image_temp = new BitmapImage(new Uri(imagesFolder, UriKind.Absolute));

                            var encoder = new JpegBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(image_temp));

                            using (var stream = new MemoryStream())
                            {
                                encoder.Save(stream);

                                var photo = new Photo()
                                {
                                    Product_Id = product.Id,
                                    Data = stream.ToArray() //byte[]
                                };

                                db.Photos.Add(photo);
                                db.SaveChanges();

                            }

                        }

                        row++; //di qua dong ke.
                        cell = tab.Cells[$"C{row}"];
                    }
                }
                MessageBox.Show("Import thanh cong.");
            }
        }

        private void loadAllImagesButton_Click(object sender, RoutedEventArgs e)
        {
            var db = new MystoreEntities();
            var photos = db.Photos.ToArray();

            allImageListView.ItemsSource = photos;

            
        }

    }
}
