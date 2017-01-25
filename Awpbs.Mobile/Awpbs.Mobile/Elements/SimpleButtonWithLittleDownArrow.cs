using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public class SimpleButtonWithLittleDownArrow : StackLayout
    {
        Label label;
        Image image;

        public event EventHandler Clicked;

        public string Text
        {
            get
            {
                return this.label.Text;
            }
            set
            {
                this.label.Text = value;
            }
        }

        public bool IsBold
        {
            get
            {
                return this.label.FontAttributes == FontAttributes.Bold;
            }
            set
            {
                this.label.FontAttributes = value ? FontAttributes.Bold : FontAttributes.None;
            }
        }

        public bool IsSmallerFont
        {
            get
            {
                return this.label.FontSize < Config.DefaultFontSize;
            }
            set
            {
                this.label.FontFamily = Config.FontFamily;
                this.label.FontSize = value ? Config.DefaultFontSize - 1 : Config.DefaultFontSize;
            }
        }

        public SimpleButtonWithLittleDownArrow(bool isBlack = true)
        {
            this.Orientation = StackOrientation.Horizontal;
            this.HeightRequest = Config.IsTablet ? 30 : 20;
            this.Spacing = 3;

            this.label = new BybLabel()
            {
                TextColor = isBlack ? Color.Black : Config.ColorTextOnBackground,//.ColorTextOnBackgroundGrayed,
                FontFamily = Config.FontFamily,
                FontSize = Config.DefaultFontSize,
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            };
            this.Children.Add(this.label);

            this.image = new Image()
            {
                Source = new FileImageSource() { File = isBlack ? "downBlack.png" : "down.png" },
                //Opacity = isBlack ? 1.0 : 0.65,
                HeightRequest = 10,
                WidthRequest = 10,
            };
            this.Children.Add(new Grid()
            {
                BackgroundColor = Color.Transparent,
                Padding = new Thickness(0,5,0,0),
				Children = 
				{
					this.image
				}
            });

            this.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    if (this.Clicked != null)
                        this.Clicked(this, EventArgs.Empty);
                }),
                NumberOfTapsRequired = 1
            });
        }
    }
}
