using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Ice_Breaking
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        DataInitializer datainit = new DataInitializer();
        ContentDialog no_student = new ContentDialog
        {
            Title = "数据库异常",
            Content = "未找到任何数据。请参阅 GitHub 项目仓库中的 README.md 修复！",
            PrimaryButtonText = "前往 GitHub",
            DefaultButton = ContentDialogButton.Primary
        };
        
        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            await datainit.InitDataAsync();
            if (datainit.person.Count == 0)
            {
                await no_student.ShowAsync();
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/HoraceHuang-ui/HDU_Ice_Breaking"));
            }
        }

        private async void start_btn_Click(object sender, RoutedEventArgs e)
        {
            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            StorageFolder storageFolder = Windows.Storage.KnownFolders.DocumentsLibrary;
            StorageFile file = await storageFolder.GetFileAsync("ice_breaking\\student_data.txt");

            string name = name_box.Text;
            string id = id_box.Text;
            int l = 0, r = datainit.person.Count - 1, m = (l + r) / 2;
            while (l <= r)
            {
                m = (l + r) / 2;
                if (datainit.person[m].id.CompareTo(id) > 0)  //id < m
                {
                    r = m - 1;
                }
                else if (datainit.person[m].id.CompareTo(id) < 0)
                {
                    l = m + 1;
                }
                else
                {
                    break;
                }
            }
            if (datainit.person[m].id != id || datainit.person[m].name != name)
            {
                invalid_name_or_id.Visibility = Visibility.Visible;
                return;
            }
            if (anonym.IsChecked == true)
            {
                if (datainit.person[m].anonym == "F")
                {
                    datainit.person[m].anonym = "T";
                    datainit.s_list[m] = serializer.Serialize(datainit.person[m]);
                    datainit.raw_str = "";
                    foreach (string str in datainit.s_list)
                    {
                        datainit.raw_str += str;
                    }
                    await FileIO.WriteTextAsync(file, datainit.raw_str);
                }
            }
            else
            {
                if (datainit.person[m].anonym == "T")
                {
                    datainit.person[m].anonym = "F";
                    datainit.s_list[m] = serializer.Serialize(datainit.person[m]);
                    datainit.raw_str = "";
                    foreach (string str in datainit.s_list)
                    {
                        datainit.raw_str += str;
                    }
                    await FileIO.WriteTextAsync(file, datainit.raw_str);
                }
            }
            invalid_name_or_id.Visibility = Visibility.Collapsed;

            file = await storageFolder.CreateFileAsync("ice_breaking\\selected.txt", CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(file, m.ToString());

            Frame.Navigate(typeof(quiz));
        }
    }
}
