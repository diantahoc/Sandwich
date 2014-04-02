using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Sandwich.WPF
{
    /// <summary>
    /// Interaction logic for ImageViewer.xaml
    /// </summary>
    public partial class ImageViewer : UserControl, TabElement
    {
        public Common.ElementType Type { get { return Common.ElementType.ImageView; } }
        public string Board { get; private set; }
        public int ID { get { return this._gp.PostNumber; } }
        public BoardBrowserWPF TParent { get; private set; }
        public string Title { get; private set; }

        string formatted_size;

        private DataTypes.FileLoader FileLoader;

        private GenericPost _gp;

        private string desktop_path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public ImageViewer(GenericPost gp, BoardBrowserWPF parent)
        {
            InitializeComponent();
            
            _gp = gp;
            
            SessionManager.RegisterFile(gp.file);

            this.TParent = parent;
            this.Board = gp.board;
            this.Title = string.Format("File: {0}.{1}", gp.file.filename, gp.file.ext);
            formatted_size = Common.format_size_string(gp.file.size);
            this.zap_c.ScrollOwner = this.zap_sc;

            FileLoader = new DataTypes.FileLoader(gp.file);
            FileLoader.FileProgressChanged += this.file_FileProgressChanged;
            FileLoader.FileLoaded += new DataTypes.FileLoader.FileLoadedEvent(FileLoader_FileLoaded);
            FileLoader.LoadImageAsync();
        }

        void FileLoader_FileLoaded()
        {
            this.Dispatcher.Invoke((Action)delegate { this.picbox.SetImage(FileLoader.Extension, FileLoader.Image); });
        }

        private void file_FileProgressChanged(double p)
        {
            double downloaded = Convert.ToDouble(_gp.file.size) * (p / 100d);

            this.progress_text.Dispatcher.Invoke((Action)delegate
            {
                this.progress_text.Content = string.Format("{0}% {1} of {2}", Math.Round(p, 2, MidpointRounding.ToEven), Common.format_size_string(downloaded), formatted_size);
                this.progressBar.Value = downloaded;
            });
        }

        private void show_message(string message, bool is_error)
        {
            if (is_error)
            {
                this.status.Foreground = Brushes.Red;
            }
            else
            {
                this.status.Foreground = Brushes.Green;
            }

            this.status.Content = message;
        }

        private void save_image(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    show_message("File already exist!", true);
                    return;
                }

                File.WriteAllBytes(path, FileLoader.ImageData);
                show_message(String.Format("{0} saved sucessfully!", FileLoader.FileName), false);
            }
            catch (Exception ex)
            {
                show_message(ex.Message, true);
            }
        }

        public void CleanUp()
        {
            this.picbox.ClearImage();
            FileLoader.Dispose();
            SessionManager.UnRegisterFile(this._gp.file);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (FileLoader.ImageData == null) { return; }
            System.Windows.Forms.SaveFileDialog sf = new System.Windows.Forms.SaveFileDialog();
            sf.AddExtension = true;
            sf.Filter = String.Format("{0} files|*.{0}", _gp.file.ext);
            sf.CheckPathExists = true;
            sf.FileName = _gp.file.filename;
            if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                save_image(sf.FileName);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (FileLoader.ImageData == null) { return; }
            string file_path = System.IO.Path.Combine(desktop_path, FileLoader.FileName);
            save_image(file_path);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (FileLoader.ImageData == null) { return; }

            string path = System.IO.Path.Combine(desktop_path, _gp.board);

            Directory.CreateDirectory(path);

            string file_path = System.IO.Path.Combine(path, FileLoader.FileName);

            save_image(file_path);
        }

        #region Pan And Zoom

        private enum MouseHandlingMode
        {
            None,
            DraggingRectangles,
            Panning,
            Zooming,
            DragZooming,
        }

        private MouseHandlingMode mouseHandlingMode = MouseHandlingMode.None;

        private Point origZoomAndPanControlMouseDownPoint;

        private Point origContentMouseDownPoint;

        private MouseButton mouseButtonDown;

        private void zoomAndPanControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            picbox.Focus();
            Keyboard.Focus(picbox);

            mouseButtonDown = e.ChangedButton;
            origZoomAndPanControlMouseDownPoint = e.GetPosition(zap_c);
            origContentMouseDownPoint = e.GetPosition(picbox);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0 &&
                (e.ChangedButton == MouseButton.Left ||
                 e.ChangedButton == MouseButton.Right))
            {
                // Shift + left- or right-down initiates zooming mode.
                mouseHandlingMode = MouseHandlingMode.Zooming;
            }
            else if (mouseButtonDown == MouseButton.Left)
            {
                // Just a plain old left-down initiates panning mode.
                mouseHandlingMode = MouseHandlingMode.Panning;
            }

            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                // Capture the mouse so that we eventually receive the mouse up event.
                zap_c.CaptureMouse();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised on mouse up in the ZoomAndPanControl.
        /// </summary>
        private void zoomAndPanControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                if (mouseHandlingMode == MouseHandlingMode.Zooming)
                {
                    if (mouseButtonDown == MouseButton.Left)
                    {
                        // Shift + left-click zooms in on the content.
                        ZoomIn(origContentMouseDownPoint);
                    }
                    else if (mouseButtonDown == MouseButton.Right)
                    {
                        // Shift + left-click zooms out from the content.
                        ZoomOut(origContentMouseDownPoint);
                    }
                }

                zap_c.ReleaseMouseCapture();
                mouseHandlingMode = MouseHandlingMode.None;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised on mouse move in the ZoomAndPanControl.
        /// </summary>
        private void zoomAndPanControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseHandlingMode == MouseHandlingMode.Panning)
            {
                //
                // The user is left-dragging the mouse.
                // Pan the viewport by the appropriate amount.
                //
                Point curContentMousePoint = e.GetPosition(picbox);
                Vector dragOffset = curContentMousePoint - origContentMouseDownPoint;

                zap_c.ContentOffsetX -= dragOffset.X;
                zap_c.ContentOffsetY -= dragOffset.Y;

                e.Handled = true;
            }
            else if (mouseHandlingMode == MouseHandlingMode.Zooming)
            {
                this.picbox.Cursor = Cursors.ScrollAll;

                e.Handled = true;
            }
            else if (mouseHandlingMode == MouseHandlingMode.DragZooming)
            {
                //
                // When in drag zooming mode continously update the position of the rectangle
                // that the user is dragging out.
                //
                Point curContentMousePoint = e.GetPosition(picbox);
                SetDragZoomRect(origContentMouseDownPoint, curContentMousePoint);

                e.Handled = true;
            }
            else { this.picbox.Cursor = Cursors.Arrow; }
        }

        /// <summary>
        /// Event raised by rotating the mouse wheel
        /// </summary>
        private void zoomAndPanControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (e.Delta > 0)
            {
                Point curContentMousePoint = e.GetPosition(picbox);
                ZoomIn(curContentMousePoint);
            }
            else if (e.Delta < 0)
            {
                Point curContentMousePoint = e.GetPosition(picbox);
                ZoomOut(curContentMousePoint);
            }
        }

        /// <summary>
        /// The 'ZoomIn' command (bound to the plus key) was executed.
        /// </summary>
        private void ZoomIn_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ZoomIn(new Point(zap_c.ContentZoomFocusX, zap_c.ContentZoomFocusY));
        }

        /// <summary>
        /// The 'ZoomOut' command (bound to the minus key) was executed.
        /// </summary>
        private void ZoomOut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ZoomOut(new Point(zap_c.ContentZoomFocusX, zap_c.ContentZoomFocusY));
        }

        /// <summary>
        /// The 'Fill' command was executed.
        /// </summary>
        private void Fill_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            zap_c.AnimatedScaleToFit();
        }

        /// <summary>
        /// The 'OneHundredPercent' command was executed.
        /// </summary>
        private void OneHundredPercent_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            zap_c.AnimatedZoomTo(1.0);
        }

        /// <summary>
        /// Zoom the viewport out, centering on the specified point (in content coordinates).
        /// </summary>
        private void ZoomOut(Point contentZoomCenter)
        {
            zap_c.ZoomAboutPoint(zap_c.ContentScale - 0.1, contentZoomCenter);
        }

        /// <summary>
        /// Zoom the viewport in, centering on the specified point (in content coordinates).
        /// </summary>
        private void ZoomIn(Point contentZoomCenter)
        {
            zap_c.ZoomAboutPoint(zap_c.ContentScale + 0.1, contentZoomCenter);
        }

        /// <summary>
        /// Update the position and size of the rectangle that user is dragging out.
        /// </summary>
        private void SetDragZoomRect(Point pt1, Point pt2)
        {
            double x, y, width, height;

            //
            // Deterine x,y,width and height of the rect inverting the points if necessary.
            // 

            if (pt2.X < pt1.X)
            {
                x = pt2.X;
                width = pt1.X - pt2.X;
            }
            else
            {
                x = pt1.X;
                width = pt2.X - pt1.X;
            }

            if (pt2.Y < pt1.Y)
            {
                y = pt2.Y;
                height = pt1.Y - pt2.Y;
            }
            else
            {
                y = pt1.Y;
                height = pt2.Y - pt1.Y;
            }

            //
            // Update the coordinates of the rectangle that is being dragged out by the user.
            // The we offset and rescale to convert from content coordinates.
            //
        }

        /// <summary>
        /// Event raised when the user has double clicked in the zoom and pan control.
        /// </summary>
        private void zoomAndPanControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
            {
                Point doubleClickPoint = e.GetPosition(picbox);
                zap_c.AnimatedSnapTo(doubleClickPoint);
            }
        }

        #endregion

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            FileLoader.LoadImageAsync(false);
        }

    }

    public class ScaleToPercentConverter : IValueConverter
    {
        /// <summary>
        /// Convert a fraction to a percentage.
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Round to an integer value whilst converting.
            return (double)(int)((double)value * 100.0);
        }

        /// <summary>
        /// Convert a percentage back to a fraction.
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (double)value / 100.0;
        }
    }
}
