using System;
using System.Collections.Generic;
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
using System.Threading;

namespace AsyncTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private const int TimerDelay = 10000;
        private DateTime TimerEnd;

        private async Task Button0_Worker()
        {
            label0.Content = "Async operation started...";
            button0.IsEnabled = false;
            button1.IsEnabled = true;

            var tsc = new TaskCompletionSource<bool>();

            button1.Click += (s, e) =>
                {
                    label1.Content = "Async operation completed...";
                    button1.IsEnabled = false;
                    tsc.TrySetResult(true);
                };

            var timer = new System.Timers.Timer(100);
            timer.Elapsed += timer_Elapsed;
            TimerEnd = DateTime.Now.AddMilliseconds(TimerDelay);
            timer.Start();

            if (tsc.Task == await Task.WhenAny(tsc.Task, Task.Delay(TimerDelay)))
            {
                await tsc.Task;
            }
            else
            {
                label1.Content = "Async opertation timed out...";
                label2.Content = "00:00";
                button1.IsEnabled = false;
                tsc.TrySetResult(false);
            }

            timer.Stop();
            button2.IsEnabled = true;
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var t = TimerEnd - DateTime.Now;
            var s = String.Format("{0}:{1}", t.ToString("ss"), t.ToString("ff"));
            RunOnUIThread(() => { label2.Content = s; });
        }

        private void RunOnUIThread(Action action)
        {
            Dispatcher.Invoke(action);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Button0_Worker();
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            label0.Content = "";
            label1.Content = "";
            label2.Content = "";
            button0.IsEnabled = true;
            button1.IsEnabled = false;
            button2.IsEnabled = false;

        }
    }
}
