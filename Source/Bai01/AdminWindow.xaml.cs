using Aspose.Cells;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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

    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Fluent.RibbonWindow
    {
        class FilterEntity
        {
            public int Value { get; set; }
        }
        public class PagingInfo
        {
            public int RowsPerPage { get; set; }
            public int CurrentPage { get; set; }
            public int TotalPages { get; set; }
            public int TotalItems { get; set; }
            public List<string> Pages
            {
                get
                {
                    var result = new List<string>();

                    for (var i = 1; i <= TotalPages; i++)
                    {
                        result.Add($"Trang {i} / {TotalPages}");
                    }

                    return result;
                }
            }
        }

        PagingInfo _pagingInfo;




        public delegate void DeathHandler();
        public event DeathHandler Dying;
        public AdminWindow()
        {
            InitializeComponent();
        }

        private void BackstageTabItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void searchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            searchHintTextBlock.Visibility = Visibility.Hidden;
        }

        private void searchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchTextBox.Text.Length == 0)
            {
                searchHintTextBlock.Visibility = Visibility.Visible;
            }
        }

        FilterEntity _filterInfo;


        private async void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _filterInfo = new FilterEntity() { Value = 80 };
            
            //statusLabel.Content = "Application is ready";

            loadingProgressBar.IsIndeterminate = true;

            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(3000);

            });

            // Gan le giao dien
            var db = new MystoreEntities();
            categoriesComboBox.ItemsSource = db.Categories.ToList();
            categoriesComboBox.SelectedIndex = 0;

            Categorie_Name_TextBox.DataContext = categoriesComboBox.SelectedItem;

            loadingProgressBar.IsIndeterminate = false;

            filterPanel.DataContext = _filterInfo;
        }

        int rowsPerPage = 4;
        int _selectedCategoryIndex = 0;
        string filename = "";
        




        void CalculatePagingInfo()
        {

            var db = new MystoreEntities();

            Categorie_Name_TextBox.DataContext = categoriesComboBox.SelectedItem;
            var selectedCategory = categoriesComboBox.SelectedItem
                as Category;

            if (selectedCategory == null )
            {
                return;
            }


            var products =
                db.Categories.Find(selectedCategory.Id)
                    .Products;
            var query = from product in products
                        where product.Photos.FirstOrDefault() != null 
                        && product.Quantity <= _filterInfo.Value
                        select new
                        {
                            ID = product.Id,
                            Quantity = product.Quantity,
                            Price = product.Price,
                            Name = product.Name,
                            Thumbnail = product.Photos
                                .FirstOrDefault().Data
                        };
            productsListView.ItemsSource = query;



            // Tinh toan thong tin phan trang
            var count = query.Count();
            _pagingInfo = new PagingInfo()
            {
                RowsPerPage = rowsPerPage,
                TotalItems = count,
                TotalPages = count / rowsPerPage +
                    (((count % rowsPerPage) == 0) ? 0 : 1),
                CurrentPage = 1
            };

            pagingComboBox.ItemsSource = _pagingInfo.Pages;
            pagingComboBox.SelectedIndex = 0;

            statusTextBlock.Text = $"Tổng sản phẩm tìm thấy: {count} ";
        }

        void UpdateProductView()
        {
            var db = new MystoreEntities();

            Categorie_Name_TextBox.DataContext = categoriesComboBox.SelectedItem;
            var selectedCategory = categoriesComboBox.SelectedItem
                as Category;

            if (selectedCategory == null)
            {
                return;
            }


            var products =
                db.Categories.Find(selectedCategory.Id)
                    .Products;
            var keyword = searchTextBox.Text;
            var query = from product in products
                        where product.Name.ToLower()
                                .Contains(keyword.ToLower())
                        where product.Photos.FirstOrDefault() != null
                        && product.Quantity <= _filterInfo.Value


                        select new
                        {
                            ID = product.Id,
                            Quantity = product.Quantity,
                            Price = product.Price,
                            Name = product.Name,
                            Thumbnail = product.Photos
                                .FirstOrDefault().Data
                        };
            productsListView.ItemsSource = query;







            var skip = (_pagingInfo.CurrentPage - 1) * _pagingInfo.RowsPerPage;
            var take = _pagingInfo.RowsPerPage;
            productsListView.ItemsSource = query.Skip(skip).Take(take);
        }

        private void pagingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int nextPage = pagingComboBox.SelectedIndex + 1;
            _pagingInfo.CurrentPage = nextPage;

            UpdateProductView();
        }

        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            var currentIndex = pagingComboBox.SelectedIndex;
            if (currentIndex > 0)
            {
                pagingComboBox.SelectedIndex--;
            }
        }

        private void keywordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculatePagingInfo();
            UpdateProductView();
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            var currentIndex = pagingComboBox.SelectedIndex;
            if (currentIndex < _pagingInfo.TotalPages)
            {
                pagingComboBox.SelectedIndex++;
            }
        }



        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculatePagingInfo();
            UpdateProductView();
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (Dying != null)
            {
                Dying.Invoke(); //Dying?.Invoke();
            }

            this.Close();
        }


        private async void load_Click(object sender, RoutedEventArgs e)
        {
            //statusTextBlock.Text = "Loading";
            loadingProgressBar.IsIndeterminate = true;
            statusTextBlock.Text = "All products is loaded";
            loadingProgressBar.IsIndeterminate = false;
        }


        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculatePagingInfo();
            UpdateProductView();
        }

        private void productsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var DL = productsListView.SelectedItem;
            ThongTinSP.DataContext = DL;
        }

        private void ADD_product_click(object sender, RoutedEventArgs e)
        {

            string image = "";
            string temp = "";

            for(int i = filename.Length - 1; i >= 0; i--)
            {
                if (filename[i] == 92 )
                {
                    break;
                }
                image += filename[i];
            }    

            for (int i = image.Length - 1; i>= 0; i--)
            {
                temp += image[i];
            }    

            var db = new MystoreEntities();
            dynamic _categories = categoriesComboBox.SelectedItem;
            var sp = new Product()
            {
                CatId = _categories.Id,
                Quantity = int.Parse(_Quantity.Text),
                Name = Ten.Text,
                Price = decimal.Parse(Gia.Text),
                Image = temp,
            };
            db.Products.Add(sp);
            

            var _image = new BitmapImage(new Uri(filename, UriKind.Absolute));

            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(_image));

            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);

                var photo = new Photo()
                {
                    Product_Id = sp.Id,
                    Data = stream.ToArray() //byte[]
                };

                db.Photos.Add(photo);
                db.SaveChanges();


            }
            CalculatePagingInfo();
            UpdateProductView();
            MessageBox.Show("Thêm sản phẩm thành công");
        }

        private void Edit_product_click(object sender, RoutedEventArgs e)
        {
            string image = "";
            string temp = "";

            for (int i = filename.Length - 1; i >= 0; i--)
            {
                if (filename[i] == 92)
                {
                    break;
                }
                image += filename[i];
            }

            for (int i = image.Length - 1; i >= 0; i--)
            {
                temp += image[i];
            }

            var db = new MystoreEntities();
            dynamic sp = productsListView.SelectedItem;
            var product = db.Products.ToList();
            var _photo = db.Photos.ToList();

            for (int i = 0; i < product.Count; i++)
            {
                if (product[i].Id == sp.ID)
                {
                    product[i].Name = Ten.Text;
                    product[i].Quantity = int.Parse(_Quantity.Text);
                    product[i].Price = decimal.Parse(Gia.Text);
                    product[i].Image = temp;
                    break;
                }
            }

            var _image = new BitmapImage(new Uri(filename, UriKind.Absolute));

            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(_image));

            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);

                for (int i = 0; i < _photo.Count; i++)
                {
                    if (_photo[i].Product_Id == sp.ID)
                    {
                        _photo[i].Data = stream.ToArray(); //byte[]
                        break;
                    }
                }
                db.SaveChanges();
            }

            

            CalculatePagingInfo();
            UpdateProductView();

            MessageBox.Show("update sản phẩm thành công");

        }

        private void Delete_product_Click(object sender, RoutedEventArgs e)
        {
            var db = new MystoreEntities();
            dynamic sp = productsListView.SelectedItem;
            var photo = db.Photos.ToList();
            var product = db.Products.ToList();
            for (int i = 0; i < photo.Count; i++)
            {
                if (photo[i].Product_Id == sp.ID)
                {
                    db.Photos.Remove(photo[i]);
                    break;
                }
            }

            for (int i = 0; i < product.Count; i++)
            {
                if (product[i].Id == sp.ID)
                {
                    db.Products.Remove(product[i]);
                    break;
                }
            }
            CalculatePagingInfo();
            db.SaveChanges();
            CalculatePagingInfo();
            UpdateProductView();
            MessageBox.Show("Xóa sản phẩm thành công");
        }



        private void addCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var db = new MystoreEntities();
            //var id = categoriesComboBox.Items.Count;
            var sp = new Category()
            {
                Name = Categorie_Name_TextBox.Text
            };
            db.Categories.Add(sp);
            db.SaveChanges();
            MessageBox.Show("Thêm hạng mục thành công");
            categoriesComboBox.ItemsSource = db.Categories.ToList();
            categoriesComboBox.SelectedIndex = 0;

        }

        private void editCategory_click(object sender, RoutedEventArgs e)
        {
            var db = new MystoreEntities();
            var sp = categoriesComboBox.SelectedItem as Category;
            var category = db.Categories.ToList();

            for (int i = 0; i < category.Count; i++)
            {
                if (category[i].Id == sp.Id)
                {
                    category[i].Name = Categorie_Name_TextBox.Text;
                    break;
                }
            }
            db.SaveChanges();
            MessageBox.Show("update hạng mục thành công");
        }

        private void deleteCategory_Click(object sender, RoutedEventArgs e)
        {
            var _productid = 0;
            var db = new MystoreEntities();
            var sp = categoriesComboBox.SelectedItem as Category;
            var product = db.Products.ToList();
            var category = db.Categories.ToList();
            var photo = db.Photos.ToList();
            for (int i = 0; i < product.Count; i++)
            {
                if (product[i].CatId == sp.Id)
                {
                    _productid = product[i].Id;
                    product[i].CatId = null;
                    if (_productid < photo.Count)
                    {
                        for (int j = 0; j < photo.Count; ++j)
                        {
                            if (photo[i].Product_Id == _productid)
                            {
                                photo[i].Product_Id = null;
                            }
                        }
                    }    
                    
                }
            }

            


            for (int i = 0; i < category.Count; i++)
            {
                if (category[i].Id == sp.Id)
                {
                    db.Categories.Remove(category[i]);
                    break;
                }
            }
            db.SaveChanges();
            categoriesComboBox.ItemsSource = db.Categories.ToList();
            categoriesComboBox.SelectedIndex = 0;
            MessageBox.Show("Xóa hạng mục thành công");
        }

        private void importExcel_Click(object sender, RoutedEventArgs e)
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


        private void ChooseImageButton_Click(object sender, RoutedEventArgs e)
        {


            var screen = new OpenFileDialog();
            if (screen.ShowDialog() == true)
            {
                filename = screen.FileName;
            }
        }

        private void priceRangeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CalculatePagingInfo();
            UpdateProductView();
        }
    }
}
