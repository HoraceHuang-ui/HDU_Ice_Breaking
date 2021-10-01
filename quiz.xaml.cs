using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;

namespace Ice_Breaking
{
    public sealed partial class quiz : Page
    {
        DataInitializer datainit = new DataInitializer();
        int selected = 0;
        string selected_raw_str = "";
        int ans_abcd = 0;  //每次生成的答案
        int remaining;  //去掉登陆人和匿名人后剩余人数
        public quiz()
        {
            this.InitializeComponent();
        }

        ContentDialog insufficient = new ContentDialog()
        {
            Title = "尴尬了……",
            Content = "本班未设置匿名的同学不足，无法启动破冰。",
            PrimaryButtonText = "确定",
            SecondaryButtonText = "不确定也得确定",
            DefaultButton = ContentDialogButton.Primary
        };
        void swap<T>(ref T a, ref T b)
        {
            T t;
            t = a;
            a = b;
            b = t;
        }
        public int[] GenerateUniqueRandom(int minValue, int maxValue, int n)
        {
            if (n > maxValue - minValue + 1)
                n = maxValue - minValue + 1;

            int maxIndex = maxValue - minValue + 2;
            int[] indexArr = new int[maxIndex];
            for (int i = 0; i < maxIndex; i++)
            {
                indexArr[i] = minValue - 1;
                minValue++;
            }

            Random ran = new Random();
            int[] randNum = new int[n];
            int index;
            for (int j = 0; j < n; j++)
            {
                index = ran.Next(1, maxIndex - 1);
                randNum[j] = indexArr[index];
                indexArr[index] = indexArr[maxIndex - 1];
                maxIndex--;
            }
            return randNum;
        }

        public void set_ans_style(ref Button chosen)
        {
            var color = new Windows.UI.Color();
            color.R = 200;
            color.G = 0;
            color.B = 0;
            color.A = 80;
            chosen.Background = new SolidColorBrush(color);  //标红错误答案

            color.R = 0;
            color.G = 200;
            switch (ans_abcd)
            {
                case 0: choice_a.Background = new SolidColorBrush(color); break;
                case 1: choice_b.Background = new SolidColorBrush(color); break;
                case 2: choice_c.Background = new SolidColorBrush(color); break;
                case 3: choice_d.Background = new SolidColorBrush(color); break;
                default: break;
            }
        }
        public void reset_ans_style()
        {
            var color = new Windows.UI.Color();
            color.A = 0;
            choice_a.Background = new SolidColorBrush(color);
            choice_b.Background = new SolidColorBrush(color);
            choice_c.Background = new SolidColorBrush(color);
            choice_d.Background = new SolidColorBrush(color);
        }
        public void get_selected()
        {
            selected = int.Parse(selected_raw_str);

            Person t = new Person();
            t = datainit.person[selected];
            datainit.person[selected] = datainit.person[datainit.person.Count - 1];
            datainit.person[datainit.person.Count - 1] = t;
        }

        //0123   ABCD
        public void refresh()
        {
            int[] choices = GenerateUniqueRandom(0, remaining - 1, 4);
            ans_abcd = GenerateUniqueRandom(0, 3, 1)[0];
            //datainit.person[choices[ans_abcd]]
            profile_photo.Source = new BitmapImage(new Uri(datainit.person[choices[ans_abcd]].photo));
            choice_a.Content = "A. " + datainit.person[choices[0]].name;
            choice_b.Content = "B. " + datainit.person[choices[1]].name;
            choice_c.Content = "C. " + datainit.person[choices[2]].name;
            choice_d.Content = "D. " + datainit.person[choices[3]].name;
            reset_ans_style();
        }

        public void move_anonym()
        {
            int i;
            for (i = 0; i < remaining; i++)
            {
                if (datainit.person[i].anonym == "T")
                {
                    Person t = new Person();
                    t = datainit.person[i];
                    datainit.person[i] = datainit.person[remaining - 1];
                    datainit.person[remaining - 1] = t;
                    remaining--;
                }
            }
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
            StorageFile file = await storageFolder.GetFileAsync("ice_breaking\\selected.txt");
            selected_raw_str = await FileIO.ReadTextAsync(file);
            await datainit.init_data_async();
            get_selected();
            remaining = datainit.person.Count - 1;
            move_anonym();
            if (remaining < 4)
            {
                await insufficient.ShowAsync();
                Frame.Navigate(typeof(MainPage));
                return;
            }
            refresh();
        }

        private void choice_a_Click(object sender, RoutedEventArgs e)
        {
            if (ans_abcd == 0)
            {
                refresh();
            }
            else
            {
                set_ans_style(ref choice_a);
            }
        }

        private void choice_b_Click(object sender, RoutedEventArgs e)
        {
            if (ans_abcd == 1)
            {
                refresh();
            }
            else
            {
                set_ans_style(ref choice_b);
            }
        }

        private void choice_c_Click(object sender, RoutedEventArgs e)
        {
            if (ans_abcd == 2)
            {
                refresh();
            }
            else
            {
                set_ans_style(ref choice_c);
            }
        }

        private void choice_d_Click(object sender, RoutedEventArgs e)
        {
            if (ans_abcd == 4)
            {
                refresh();
            }
            else
            {
                set_ans_style(ref choice_d);
            }
        }

        private void home_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
