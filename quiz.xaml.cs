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
        private int Min(int a, int b)
        {
            return (a < b) ? a : b;
        }
        private int[] GenerateUniqueRandom(int minValue, int maxValue, int n)
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

        private void SetAnswerStyle(ref Button chosen)
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
        private void ResetAnswerStyle()
        {
            var color = new Windows.UI.Color();
            color.A = 0;
            choice_a.Background = new SolidColorBrush(color);
            choice_b.Background = new SolidColorBrush(color);
            choice_c.Background = new SolidColorBrush(color);
            choice_d.Background = new SolidColorBrush(color);
        }
        private void GetSelected()
        {
            selected = int.Parse(selected_raw_str);
        }

        //0123   ABCD
        private void Refresh()
        {
            int[] choices = { -1, -1, -1, -1 };
            ans_abcd = GenerateUniqueRandom(0, 3, 1)[0];
        ReGenerate:
            choices[ans_abcd] = GenerateUniqueRandom(0, remaining - 1, 1)[0];
            if (choices[ans_abcd] == selected) goto ReGenerate;
            int i, j, k;
            int[] random_male = GenerateUniqueRandom(0, datainit.Male.Count - 1, Min(6, datainit.Male.Count));
            int[] random_female = GenerateUniqueRandom(0, datainit.Female.Count - 1, Min(6, datainit.Female.Count));
            for (i = 0; i < random_male.Length; i++)
                random_male[i] = datainit.Male[random_male[i]];
            for (i = 0; i < random_female.Length; i++)
                random_female[i] = datainit.Female[random_female[i]];

            bool m = datainit.person[choices[ans_abcd]].male == "T";
            int[] set = m ? random_male : random_female;
            int[] alt_set = m ? random_female : random_male;
            profile_photo.Source = new BitmapImage(new Uri(datainit.person[choices[ans_abcd]].photo));

            j = 0;
            for (i = 0; i < Min(4, set.Length) && j < set.Length; i++)
            {
                if (choices[i] != -1) continue;
                if (choices.Contains(set[j]) || set[j] == selected)
                {
                    j++;
                    i--;
                    continue;
                }
                choices[i] = set[j];
                j++;
            }
            if (i < 4)
            {
                k = i;
                j = 0;
                for (i = 0; i < 4 - k; i++)
                {
                    if (choices[k + i] != -1) continue;
                    if (choices.Contains(alt_set[j]) || alt_set[j] == selected)
                    {
                        j++;
                        i--;
                        continue;
                    }
                    choices[k + i] = alt_set[j];
                    j++;
                }
            }
            choice_a.Content = "A. " + datainit.person[choices[0]].name;
            choice_b.Content = "B. " + datainit.person[choices[1]].name;
            choice_c.Content = "C. " + datainit.person[choices[2]].name;
            choice_d.Content = "D. " + datainit.person[choices[3]].name;
            ResetAnswerStyle();
        }

        private void MoveAnonymPerson()
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
            await datainit.InitDataAsync();
            GetSelected();
            remaining = datainit.person.Count - 1;
            MoveAnonymPerson();
            if (remaining < 4)
            {
                await insufficient.ShowAsync();
                Frame.Navigate(typeof(MainPage));
                return;
            }
            Refresh();
        }

        private void choice_a_Click(object sender, RoutedEventArgs e)
        {
            if (ans_abcd == 0)
            {
                Refresh();
            }
            else
            {
                SetAnswerStyle(ref choice_a);
            }
        }

        private void choice_b_Click(object sender, RoutedEventArgs e)
        {
            if (ans_abcd == 1)
            {
                Refresh();
            }
            else
            {
                SetAnswerStyle(ref choice_b);
            }
        }

        private void choice_c_Click(object sender, RoutedEventArgs e)
        {
            if (ans_abcd == 2)
            {
                Refresh();
            }
            else
            {
                SetAnswerStyle(ref choice_c);
            }
        }

        private void choice_d_Click(object sender, RoutedEventArgs e)
        {
            if (ans_abcd == 4)
            {
                Refresh();
            }
            else
            {
                SetAnswerStyle(ref choice_d);
            }
        }

        private void home_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
