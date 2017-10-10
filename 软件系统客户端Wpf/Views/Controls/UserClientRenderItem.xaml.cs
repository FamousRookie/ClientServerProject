﻿using ClientsLibrary;
using CommonLibrary;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 软件系统客户端Wpf.Views.Controls
{
    /// <summary>
    /// UserClientRenderItem.xaml 的交互逻辑
    /// </summary>
    public partial class UserClientRenderItem : UserControl
    {
        public UserClientRenderItem()
        {
            InitializeComponent();
        }


        public string UniqueId
        {
            get
            {
                return netAccount == null ? string.Empty : netAccount.UniqueId;
            }
        }


        public void SetClientRender(NetAccount account)
        {
            if (account != null)
            {
                netAccount = account;
                UserName.Text = string.IsNullOrEmpty(account.Alias) ? account.UserName : account.Alias;
                Factory.Text = $"({account.Factory})";

                Roles.Children.Clear();
                if(account.Roles?.Length>0)
                {
                    foreach(var m in account.Roles)
                    {
                        TextBlock block = new TextBlock();
                        block.Background = Brushes.LightSkyBlue;
                        block.Foreground = Brushes.Blue;
                        block.Margin = new Thickness(0, 0, 4, 0);

                        block.Text = m;

                        Roles.Children.Add(block);
                    }
                }
                else
                {
                    Roles.Children.Add(new TextBlock());
                }

                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ThreadPoolLoadPortrait), account);

            }
        }


        public void UpdatePortrait(string userName)
        {
            if(netAccount?.UserName == userName)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ThreadPoolLoadPortrait), netAccount);
            }
        }
        private void ThreadPoolLoadPortrait(object obj)
        {
            // 向服务器请求小头像
            if (obj is NetAccount m_NetAccount)
            {
                try
                {
                    System.Drawing.Bitmap bitmap = UserPortrait.DownloadSmallPortraint(m_NetAccount.UserName);
                    MemoryStream ms = new MemoryStream();
                    bitmap.Save(ms, bitmap.RawFormat);
                    bitmap.Dispose();

                    Dispatcher.Invoke(new Action(() =>
                    {
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.StreamSource = ms;
                        bi.EndInit();
                        Image1.Source = bi;
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        private NetAccount netAccount = null;
    }
}
