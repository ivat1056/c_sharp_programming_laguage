using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MinesweeperCalculator
{
    /// <summary>
    /// Calculator-themed minesweeper window with visual effects.
    /// </summary>
    public partial class CalculatorWindow : Window
    {
        private const int SmallWindowWidth = 340;
        private const int SmallWindowHeight = 610;
        private const int LargeWindowWidth = 500;
        private const int LargeWindowHeight = 800;
        private const int DefaultButtonSize = 65;
        private const int ExpandedButtonSize = 90;
        private const int DoubleButtonExtraWidth = 8;
        private const int Rows = 5;
        private const int Columns = 4;
        private const int Mines = 10;
        private const double RotationAnimationDuration = 0.7;
        private const double FadeAnimationDuration = 0.7;
        private const double OopsFadeInDuration = 0.5;
        private const double OopsDisplayDuration = 2.5;
        private const double ExpandAnimationDuration = 0.5;
        private const double ScatterMinDistance = 300.0;
        private const double ScatterDistanceRange = 400.0;
        private const double ScatterMinDuration = 0.8;
        private const double ScatterDurationRange = 0.4;
        private const double FadeOutMinDuration = 0.6;
        private const double FadeOutDurationRange = 0.3;
        private const double ScatterRotationRange = 720.0;
        private const double ExpandedButtonFontSize = 38;
        private const double DefaultButtonFontSize = 28;
        private const double ExpandedRotationAngle = 180;
        private const double MinimizedRotationAngle = 90;
        private const double DefaultRotationAngle = 0;
        private const double CollapseDelaySeconds = 0.8;
        private const double ResetRotationDuration = 0.3;

        private readonly string[] calculatorLabels = { "C", "±", "%", "÷", "7", "8", "9", "×", "4", "5", "6", "-", "1", "2", "3", "+", "0", "0", ".", "=" };

        private static readonly SolidColorBrush FlagBackground = new SolidColorBrush(Color.FromRgb(255, 200, 100));
        private static readonly SolidColorBrush FlagForeground = new SolidColorBrush(Colors.Black);
        private static readonly SolidColorBrush MineBackground = new SolidColorBrush(Colors.DarkRed);
        private static readonly SolidColorBrush MineForeground = new SolidColorBrush(Colors.White);
        private static readonly SolidColorBrush SpecialRevealedBackground = new SolidColorBrush(Color.FromRgb(255, 200, 150));
        private static readonly SolidColorBrush RevealedBackground = new SolidColorBrush(Color.FromRgb(100, 100, 100));
        private static readonly SolidColorBrush OperatorBackground = new SolidColorBrush(Color.FromRgb(255, 149, 0));
        private static readonly SolidColorBrush OperatorForeground = new SolidColorBrush(Colors.White);
        private static readonly SolidColorBrush FunctionBackground = new SolidColorBrush(Color.FromRgb(166, 166, 166));
        private static readonly SolidColorBrush FunctionForeground = new SolidColorBrush(Colors.Black);
        private static readonly SolidColorBrush NumberBackground = new SolidColorBrush(Color.FromRgb(51, 51, 51));
        private static readonly SolidColorBrush NumberForeground = new SolidColorBrush(Colors.White);

        private readonly stuff gameBoard;
        private Button?[,]? cellButtons;
        private bool isExpanded = false;

        public CalculatorWindow()
        {
            InitializeComponent();
            gameBoard = new stuff(Rows, Columns, Mines);
            gameBoard.BoardStateChanged += UpdateBoardState;
            BuildBoard();
        }

        private void BuildBoard()
        {
            GameGrid.Children.Clear();
            cellButtons = new Button?[Rows, Columns];

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    Button? button = CreateCellButton(row, col);
                    if (button == null)
                    {
                        continue;
                    }

                    cellButtons[row, col] = button;
                    GameGrid.Children.Add(button);
                }
            }

            gameBoard.ResetBoard();
        }

        private Button? CreateCellButton(int row, int col)
        {
            if (row == Rows - 1 && col == 1)
            {
                return null;
            }

            Button button = new Button();
            string label = GetLabelForPosition(row, col);

            if (row == Rows - 1 && col == 0)
            {
                button.Content = "0";
                Grid.SetColumnSpan(button, 2);
                button.Width = DefaultButtonSize * 2 + DoubleButtonExtraWidth;
            }
            else
            {
                button.Content = label;
            }

            Grid.SetRow(button, row);
            Grid.SetColumn(button, col);

            bool isOperatorButton = IsOperatorButton(row, col, label);
            bool isFunctionButton = IsFunctionButton(row, col);
            bool isEqualsButton = IsEqualsButton(label);

            if (isOperatorButton || isEqualsButton)
            {
                button.Style = (Style)FindResource("OperatorButtonStyle");
            }
            else if (isFunctionButton)
            {
                button.Style = (Style)FindResource("FunctionButtonStyle");
            }
            else
            {
                button.Style = (Style)FindResource("NumberButtonStyle");
            }

            button.Tag = (row, col);
            button.MouseRightButtonDown += OnCellRightClick;
            button.Click += OnCellClick;
            button.RenderTransformOrigin = new Point(0.5, 0.5);
            button.RenderTransform = new RotateTransform(0);

            return button;
        }

        private string GetLabelForPosition(int row, int col)
        {
            if (row == Rows - 1 && col == 1)
            {
                return string.Empty;
            }

            int index = row * Columns + col;
            return index < calculatorLabels.Length ? calculatorLabels[index] : string.Empty;
        }

        private bool IsOperatorButton(int row, int col, string label)
        {
            return col == Columns - 1 && label != string.Empty;
        }

        private bool IsFunctionButton(int row, int col)
        {
            return row == 0 && col < Columns - 1;
        }

        private static bool IsEqualsButton(string label)
        {
            return label == "=";
        }

        private static IEasingFunction CreateEaseInOutEasing()
        {
            return new CubicEase { EasingMode = EasingMode.EaseInOut };
        }

        private static IEasingFunction CreateEaseOutEasing()
        {
            return new CubicEase { EasingMode = EasingMode.EaseOut };
        }

        private static IEasingFunction CreateEaseInEasing()
        {
            return new CubicEase { EasingMode = EasingMode.EaseIn };
        }

        private void OnCellClick(object sender, RoutedEventArgs e)
        {
            Button? clickedButton = sender as Button;
            if (clickedButton == null)
            {
                return;
            }

            var (row, col) = ((int, int))clickedButton.Tag;

            if (row == 0 && col == 0)
            {
                BuildBoard();
                return;
            }

            gameBoard.RevealCell(row, col);

            if (gameBoard.IsGameOver == true)
            {
                RevealMines();
            }
            else
            {
                if (gameBoard.HasWon == true)
                {
                }
            }
        }

        private void OnCellRightClick(object sender, MouseButtonEventArgs e)
        {
            Button? clickedButton = sender as Button;
            if (clickedButton == null)
            {
                return;
            }

            var (row, col) = ((int, int))clickedButton.Tag;
            gameBoard.ToggleFlag(row, col);
            e.Handled = true;
        }

        private void UpdateBoardState()
        {
            if (cellButtons == null)
            {
                return;
            }

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (row == Rows - 1 && col == 1)
                    {
                        continue;
                    }

                    Button? button = cellButtons[row, col];
                    if (button == null)
                    {
                        continue;
                    }

                    string label = GetLabelForPosition(row, col);
                    bool isOperatorButton = IsOperatorButton(row, col, label);
                    bool isFunctionButton = IsFunctionButton(row, col);
                    bool isEqualsButton = IsEqualsButton(label);
                    bool isSpecialButton = isOperatorButton || isFunctionButton || isEqualsButton;

                    if (gameBoard.IsCellFlagged(row, col))
                    {
                        ApplyFlagAppearance(button);
                    }
                    else if (gameBoard.IsCellRevealed(row, col))
                    {
                        ApplyRevealedAppearance(button, label, isSpecialButton, row, col);
                    }
                    else
                    {
                        ApplyHiddenAppearance(button, label, isOperatorButton, isFunctionButton, isEqualsButton);
                    }
                }
            }

            DisplayLabel.Text = gameBoard.GetRemainingMines().ToString();

            if (gameBoard.IsGameOver)
            {
                DisplayLabel.Text = "ERROR";
                DisplayLabel.Foreground = new SolidColorBrush(Colors.Red);
                ResetButton.Visibility = Visibility.Visible;
                ScatterButtonsOnError();
            }
            else if (gameBoard.HasWon)
            {
                DisplayLabel.Text = "WIN";
                DisplayLabel.Foreground = new SolidColorBrush(Colors.Green);
                ResetButton.Visibility = Visibility.Visible;
            }
            else
            {
                DisplayLabel.Foreground = new SolidColorBrush(Colors.White);
                ResetButton.Visibility = Visibility.Collapsed;
            }
        }

        private void ApplyHiddenAppearance(Button button, string label, bool isOperatorButton, bool isFunctionButton, bool isEqualsButton)
        {
            button.Content = label;

            if (isOperatorButton || isEqualsButton)
            {
                button.Background = OperatorBackground;
                button.Foreground = OperatorForeground;
            }
            else if (isFunctionButton)
            {
                button.Background = FunctionBackground;
                button.Foreground = FunctionForeground;
            }
            else
            {
                button.Background = NumberBackground;
                button.Foreground = NumberForeground;
            }

            button.IsEnabled = true;
        }

        private void ApplyRevealedAppearance(Button button, string label, bool isSpecialButton, int row, int col)
        {
            int value = gameBoard.GetCellValue(row, col);
            if (value == -1)
            {
                button.Content = "E";
                button.Background = MineBackground;
                button.Foreground = MineForeground;
            }
            else if (value == 0)
            {
                button.Content = label;
                button.Background = isSpecialButton ? SpecialRevealedBackground : RevealedBackground;
                button.Foreground = NumberForeground;
            }
            else
            {
                if (isSpecialButton)
                {
                    button.Content = label;
                    button.Background = SpecialRevealedBackground;
                    button.Foreground = NumberForeground;
                }
                else
                {
                    button.Content = value.ToString();
                    button.Background = RevealedBackground;
                    button.Foreground = GetNumberBrush(value);
                }
            }

            button.IsEnabled = false;
        }

        private void ApplyFlagAppearance(Button button)
        {
            button.Content = "±";
            button.Background = FlagBackground;
            button.Foreground = FlagForeground;
            button.IsEnabled = true;
        }

        private Brush GetNumberBrush(int value)
        {
            return value switch
            {
                1 => new SolidColorBrush(Colors.Blue),
                2 => new SolidColorBrush(Colors.Green),
                3 => new SolidColorBrush(Colors.Red),
                4 => new SolidColorBrush(Colors.DarkBlue),
                5 => new SolidColorBrush(Colors.DarkRed),
                6 => new SolidColorBrush(Colors.Teal),
                7 => new SolidColorBrush(Colors.Black),
                8 => new SolidColorBrush(Colors.Gray),
                _ => new SolidColorBrush(Colors.Black)
            };
        }

        private void RevealMines()
        {
            var buttons = cellButtons;
            if (buttons == null)
            {
                return;
            }

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    Button? targetButton = buttons[row, col];
                    if (targetButton == null)
                    {
                        continue;
                    }

                    int value = gameBoard.GetCellValue(row, col);
                    if (value == -1)
                    {
                        targetButton.Content = "E";
                        targetButton.Background = MineBackground;
                        targetButton.Foreground = MineForeground;
                        targetButton.IsEnabled = false;
                    }
                }
            }
        }

        private void NewGameButton_Click(object t1, RoutedEventArgs t2)
        {
            BuildBoard();
        }

        private void ResetButton_Click(object t1, RoutedEventArgs t2)
        {
            ResetButtonTransforms();
            BuildBoard();
            ResetButton.Visibility = Visibility.Collapsed;
        }

        private void MinimizeButton_Click(object t1, RoutedEventArgs t2)
        {
            this.Width = LargeWindowWidth;
            this.Height = LargeWindowHeight;

            RotateTransform? v1 = this.FindName("WindowRotateTransform") as RotateTransform;
            if (v1 == null)
            {
                v1 = this.RenderTransform as RotateTransform;
            }

            if (v1 != null)
            {
                DoubleAnimation v2 = new DoubleAnimation();
                v2.From = DefaultRotationAngle;
                v2.To = MinimizedRotationAngle;
                v2.Duration = new Duration(TimeSpan.FromSeconds(RotationAnimationDuration));
                v2.EasingFunction = CreateEaseInOutEasing();
                v1.BeginAnimation(RotateTransform.AngleProperty, v2);
            }

            FadeOutControls();

            System.Windows.Threading.DispatcherTimer v3 = new System.Windows.Threading.DispatcherTimer();
            v3.Interval = TimeSpan.FromSeconds(CollapseDelaySeconds);
            v3.Tick += (s, args) =>
            {
                v3.Stop();
                ShowOopsSequence();
            };
            v3.Start();
        }

        private void FadeOutControls()
        {
            var buttons = cellButtons;
            if (buttons == null)
            {
                return;
            }

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    Button? button = buttons[row, col];
                    if (button == null)
                    {
                        continue;
                    }

                    DoubleAnimation fadeOutAnimation = new DoubleAnimation();
                    fadeOutAnimation.From = 1.0;
                    fadeOutAnimation.To = 0.0;
                    fadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(FadeAnimationDuration));
                    fadeOutAnimation.EasingFunction = CreateEaseOutEasing();
                    button.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                }
            }

            DoubleAnimation minimizeFade = new DoubleAnimation();
            minimizeFade.From = 1.0;
            minimizeFade.To = 0.0;
            minimizeFade.Duration = new Duration(TimeSpan.FromSeconds(FadeAnimationDuration));
            MinimizeButton.BeginAnimation(UIElement.OpacityProperty, minimizeFade);

            DoubleAnimation expandFade = new DoubleAnimation();
            expandFade.From = 1.0;
            expandFade.To = 0.0;
            expandFade.Duration = new Duration(TimeSpan.FromSeconds(FadeAnimationDuration));
            ExpandButton.BeginAnimation(UIElement.OpacityProperty, expandFade);

            if (ResetButton.Visibility == Visibility.Visible)
            {
                DoubleAnimation resetFade = new DoubleAnimation();
                resetFade.From = 1.0;
                resetFade.To = 0.0;
                resetFade.Duration = new Duration(TimeSpan.FromSeconds(FadeAnimationDuration));
                ResetButton.BeginAnimation(UIElement.OpacityProperty, resetFade);
            }

            DoubleAnimation displayFade = new DoubleAnimation();
            displayFade.From = 1.0;
            displayFade.To = 0.0;
            displayFade.Duration = new Duration(TimeSpan.FromSeconds(FadeAnimationDuration));
            DisplayLabel.BeginAnimation(UIElement.OpacityProperty, displayFade);
        }

        private void ShowOopsSequence()
        {
            OopsLabel.Visibility = Visibility.Visible;

            DoubleAnimation aa1 = new DoubleAnimation();
            aa1.From = 0.0;
            aa1.To = 1.0;
            aa1.Duration = new Duration(TimeSpan.FromSeconds(OopsFadeInDuration));
            aa1.EasingFunction = CreateEaseInEasing();
            OopsLabel.BeginAnimation(UIElement.OpacityProperty, aa1);

            System.Windows.Threading.DispatcherTimer aa2 = new System.Windows.Threading.DispatcherTimer();
            aa2.Interval = TimeSpan.FromSeconds(OopsDisplayDuration);
            aa2.Tick += (s, args) =>
            {
                aa2.Stop();
                RestoreAfterOops();
            };
            aa2.Start();
        }

        private void RestoreAfterOops()
        {
            OopsLabel.Visibility = Visibility.Collapsed;
            OopsLabel.Opacity = 0;

            RotateTransform? windowRotation = this.FindName("WindowRotateTransform") as RotateTransform;
            if (windowRotation == null)
            {
                windowRotation = this.RenderTransform as RotateTransform;
            }

            if (windowRotation != null)
            {
                DoubleAnimation rotationAnimation = new DoubleAnimation();
                rotationAnimation.From = MinimizedRotationAngle;
                rotationAnimation.To = DefaultRotationAngle;
                rotationAnimation.Duration = new Duration(TimeSpan.FromSeconds(RotationAnimationDuration));
                rotationAnimation.EasingFunction = CreateEaseInOutEasing();
                windowRotation.BeginAnimation(RotateTransform.AngleProperty, rotationAnimation);
            }

            if (!isExpanded)
            {
                this.Width = SmallWindowWidth;
                this.Height = SmallWindowHeight;
            }

            var buttons = cellButtons;
            if (buttons != null)
            {
                for (int row = 0; row < Rows; row++)
                {
                    for (int col = 0; col < Columns; col++)
                    {
                        Button? button = buttons[row, col];
                        if (button != null)
                        {
                            button.Opacity = 1.0;
                            button.BeginAnimation(UIElement.OpacityProperty, null);
                        }
                    }
                }
            }

            MinimizeButton.Opacity = 1.0;
            MinimizeButton.BeginAnimation(UIElement.OpacityProperty, null);
            ExpandButton.Opacity = 1.0;
            ExpandButton.BeginAnimation(UIElement.OpacityProperty, null);
            if (ResetButton.Visibility == Visibility.Visible)
            {
                ResetButton.Opacity = 1.0;
                ResetButton.BeginAnimation(UIElement.OpacityProperty, null);
            }
            DisplayLabel.Opacity = 1.0;
            DisplayLabel.BeginAnimation(UIElement.OpacityProperty, null);
        }

        private void ExpandButton_Click(object t1, RoutedEventArgs t2)
        {
            if (isExpanded)
            {
                this.Width = SmallWindowWidth;
                this.Height = SmallWindowHeight;
                ExpandButton.Content = "⛶";
                ExpandButton.ToolTip = "Расширить окно";
                isExpanded = false;
                AnimateButtonsForExpansion(DefaultRotationAngle, DefaultButtonSize);
            }
            else
            {
                this.Width = LargeWindowWidth;
                this.Height = LargeWindowHeight;
                ExpandButton.Content = "⊟";
                ExpandButton.ToolTip = "Свернуть окно";
                isExpanded = true;
                AnimateButtonsForExpansion(ExpandedRotationAngle, ExpandedButtonSize);
            }
        }

        private void AnimateButtonsForExpansion(double targetAngle, int targetSize)
        {
            var buttons = cellButtons;
            if (buttons == null)
            {
                return;
            }

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    Button? hh1 = buttons[row, col];
                    if (hh1 != null)
                    {
                        RotateTransform? hh2 = hh1.RenderTransform as RotateTransform;
                        if (hh2 == null)
                        {
                            hh2 = new RotateTransform();
                            hh1.RenderTransformOrigin = new Point(0.5, 0.5);
                            hh1.RenderTransform = hh2;
                        }

                        DoubleAnimation ii1 = new DoubleAnimation();
                        ii1.From = hh2.Angle;
                        ii1.To = targetAngle;
                        ii1.Duration = new Duration(TimeSpan.FromSeconds(ExpandAnimationDuration));
                        ii1.EasingFunction = CreateEaseInOutEasing();
                        hh2.BeginAnimation(RotateTransform.AngleProperty, ii1);

                        double currentWidth = hh1.ActualWidth;
                        double targetWidth = 0;
                        if (row == Rows - 1 && col == 0)
                        {
                            targetWidth = targetSize * 2 + DoubleButtonExtraWidth;
                        }
                        else
                        {
                            targetWidth = targetSize;
                        }
                        if (currentWidth <= 0)
                        {
                            currentWidth = DefaultButtonSize;
                        }

                        DoubleAnimation ii2 = new DoubleAnimation();
                        ii2.From = currentWidth;
                        ii2.To = targetWidth;
                        ii2.Duration = new Duration(TimeSpan.FromSeconds(ExpandAnimationDuration));
                        ii2.EasingFunction = CreateEaseInOutEasing();
                        hh1.BeginAnimation(Button.WidthProperty, ii2);

                        double currentHeight = hh1.ActualHeight;
                        double targetHeight = targetSize;
                        if (currentHeight <= 0)
                        {
                            currentHeight = DefaultButtonSize;
                        }

                        DoubleAnimation ii3 = new DoubleAnimation();
                        ii3.From = currentHeight;
                        ii3.To = targetHeight;
                        ii3.Duration = new Duration(TimeSpan.FromSeconds(ExpandAnimationDuration));
                        ii3.EasingFunction = CreateEaseInOutEasing();
                        hh1.BeginAnimation(Button.HeightProperty, ii3);

                        if (targetSize == DefaultButtonSize)
                        {
                            hh1.FontSize = DefaultButtonFontSize;
                        }
                        else
                        {
                            hh1.FontSize = ExpandedButtonFontSize;
                        }
                    }
                }
            }
        }

        private void ScatterButtonsOnError()
        {
            var buttons = cellButtons;
            if (buttons == null)
            {
                return;
            }

            Random jj1 = new Random();
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    Button? animatedButton = buttons[row, col];
                    if (animatedButton != null)
                    {
                        TransformGroup transformGroup = animatedButton.RenderTransform as TransformGroup ?? new TransformGroup();
                        if (animatedButton.RenderTransform is RotateTransform existingRotate)
                        {
                            transformGroup.Children.Add(existingRotate);
                        }
                        animatedButton.RenderTransform = transformGroup;

                        TranslateTransform? translateTransform = transformGroup.Children.OfType<TranslateTransform>().FirstOrDefault();
                        if (translateTransform == null)
                        {
                            translateTransform = new TranslateTransform();
                            transformGroup.Children.Add(translateTransform);
                        }

                        double angle = jj1.NextDouble() * 2.0 * Math.PI;
                        double distance = ScatterMinDistance + jj1.NextDouble() * ScatterDistanceRange;
                        double offsetX = Math.Cos(angle) * distance;
                        double offsetY = Math.Sin(angle) * distance;

                        DoubleAnimation translateXAnimation = new DoubleAnimation();
                        translateXAnimation.From = 0;
                        translateXAnimation.To = offsetX;
                        double duration1 = ScatterMinDuration + jj1.NextDouble() * ScatterDurationRange;
                        translateXAnimation.Duration = new Duration(TimeSpan.FromSeconds(duration1));
                        translateXAnimation.EasingFunction = CreateEaseOutEasing();

                        DoubleAnimation translateYAnimation = new DoubleAnimation();
                        translateYAnimation.From = 0;
                        translateYAnimation.To = offsetY;
                        double duration2 = ScatterMinDuration + jj1.NextDouble() * ScatterDurationRange;
                        translateYAnimation.Duration = new Duration(TimeSpan.FromSeconds(duration2));
                        translateYAnimation.EasingFunction = CreateEaseOutEasing();

                        RotateTransform? rotateTransform = transformGroup.Children.OfType<RotateTransform>().FirstOrDefault();
                        if (rotateTransform == null)
                        {
                            rotateTransform = new RotateTransform();
                            transformGroup.Children.Add(rotateTransform);
                        }

                        double randomRotation = (jj1.NextDouble() - 0.5) * ScatterRotationRange;
                        DoubleAnimation rotationAnimation = new DoubleAnimation();
                        rotationAnimation.From = rotateTransform.Angle;
                        rotationAnimation.To = rotateTransform.Angle + randomRotation;
                        double duration3 = ScatterMinDuration + jj1.NextDouble() * ScatterDurationRange;
                        rotationAnimation.Duration = new Duration(TimeSpan.FromSeconds(duration3));
                        rotationAnimation.EasingFunction = CreateEaseOutEasing();
                        rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotationAnimation);

                        DoubleAnimation opacityAnimation = new DoubleAnimation();
                        opacityAnimation.From = 1.0;
                        opacityAnimation.To = 0.0;
                        double duration4 = FadeOutMinDuration + jj1.NextDouble() * FadeOutDurationRange;
                        opacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(duration4));
                        opacityAnimation.EasingFunction = CreateEaseOutEasing();
                        animatedButton.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);

                        translateTransform.BeginAnimation(TranslateTransform.XProperty, translateXAnimation);
                        translateTransform.BeginAnimation(TranslateTransform.YProperty, translateYAnimation);
                    }
                }
            }
        }

        private void ResetButtonTransforms()
        {
            var buttons = cellButtons;
            if (buttons == null)
            {
                return;
            }

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    Button? button = buttons[row, col];
                    if (button != null)
                    {
                        if (button.RenderTransform is TransformGroup transformGroup)
                        {
                            TranslateTransform? translateTransform = transformGroup.Children.OfType<TranslateTransform>().FirstOrDefault();
                            if (translateTransform != null)
                            {
                                translateTransform.BeginAnimation(TranslateTransform.XProperty, null);
                                translateTransform.BeginAnimation(TranslateTransform.YProperty, null);
                                translateTransform.X = 0;
                                translateTransform.Y = 0;
                            }

                            RotateTransform? rotateTransform = transformGroup.Children.OfType<RotateTransform>().FirstOrDefault();
                            if (rotateTransform != null)
                            {
                                double currentAngle = rotateTransform.Angle;
                                rotateTransform.BeginAnimation(RotateTransform.AngleProperty, null);
                                double targetAngle = isExpanded ? ExpandedRotationAngle : DefaultRotationAngle;

                                DoubleAnimation rotationAnimation = new DoubleAnimation();
                                rotationAnimation.From = currentAngle;
                                rotationAnimation.To = targetAngle;
                                rotationAnimation.Duration = new Duration(TimeSpan.FromSeconds(ResetRotationDuration));
                                rotationAnimation.EasingFunction = CreateEaseInOutEasing();
                                rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotationAnimation);
                            }
                        }

                        button.Opacity = 1.0;
                        button.BeginAnimation(UIElement.OpacityProperty, null);
                    }
                }
            }
        }
    }
}
