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

        private void RefreshFlyoutState()
        {
            new_name_box.Text = "";
            new_id_box.Text = "";
            dropdown_gender.Content = "性别";
            new_photo_box.Text = "";
            add_save.Content = "保存";
            add_save.IsEnabled = true;
            gender_expected.Visibility = Visibility.Collapsed;
            user_existed.Visibility = Visibility.Collapsed;
        }

        private int DivisionSearchByID(int l, int r, string arg)
        {
            int m = (l + r) / 2;
            bool found = false;
            while (l <= r)
            {
                m = (l + r) / 2;
                if (datainit.person[m].id.CompareTo(arg) > 0)  // id < m
                {
                    r = m - 1;
                }
                else if (datainit.person[m].id.CompareTo(arg) < 0)
                {
                    l = m + 1;
                }
                else
                {
                    found = true;
                    break;
                }
            }
            if (found)
                return m;
            else
                return -1;
        }

        private async void start_btn_Click(object sender, RoutedEventArgs e)
        {
            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            StorageFolder storageFolder = Windows.Storage.KnownFolders.DocumentsLibrary;
            StorageFile file = await storageFolder.GetFileAsync("ice_breaking\\student_data.txt");

            string name = name_box.Text;
            string id = id_box.Text;
            int m = DivisionSearchByID(0, datainit.person.Count - 1, id);
            if (m == -1 || datainit.person[m].name != name)
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

        private async void add_save_Click(object sender, RoutedEventArgs e)
        {
            gender_expected.Visibility = Visibility.Collapsed;
            user_existed.Visibility = Visibility.Collapsed;

            string gender = "";
            if (dropdown_gender.Content.ToString() == "男")
                gender = "T";
            else
                gender = "F";
            int m = DivisionSearchByID(0, datainit.person.Count - 1, new_id_box.Text);
            if (m == -1)
            {
                if (new_name_box.Text == "")
                {
                    new_name_box.Text = "请输入姓名！";
                    return;
                }

                bool valid_id = true;
                if (new_id_box.Text == "")
                    valid_id = false;
                for (int i = 0; i < new_id_box.Text.Length; i++)
                    if (!(new_id_box.Text[i] >= '0' && new_id_box.Text[i] <= '9'))
                    {
                        valid_id = false;
                        break;
                    }
                if (!valid_id)
                {
                    new_id_box.Text = "请输入正确学号！";
                    return;
                }

                if (dropdown_gender.Content.ToString() == "性别")
                {
                    gender_expected.Visibility = Visibility.Visible;
                    return;
                }

                if (new_photo_box.Text == "")
                {
                    new_photo_box.Text = "请输入图片地址！";
                    return;
                }
                await datainit.AddPersonAsync(new_name_box.Text, new_id_box.Text, gender, new_photo_box.Text);
                add_save.Content = "已保存。";
                add_save.IsEnabled = false;
            }
            else
            {
                user_existed.Visibility = Visibility.Visible;
            }
        }

        private void dropdown_male_Click(object sender, RoutedEventArgs e)
        {
            dropdown_gender.Content = "男";
        }

        private void dropdown_female_Click(object sender, RoutedEventArgs e)
        {
            dropdown_gender.Content = "女";
        }

        private void add_user_Click(object sender, RoutedEventArgs e)
        {
            RefreshFlyoutState();
        }
    }
}
