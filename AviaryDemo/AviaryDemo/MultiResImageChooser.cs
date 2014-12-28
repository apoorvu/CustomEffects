using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AviaryDemo
{
    public class MultiResImageChooser
    {
        public Uri BestResolutionImage
        {
            get
            {
                switch (ResolutionHelper.CurrentResolution)
                {
                    case Resolutions.HD720p:
                        return new Uri("Assets/back720p.jpg", UriKind.Relative);
                    case Resolutions.WXGA:
                        return new Uri("Assets/backWVGA.jpg", UriKind.Relative);
                    case Resolutions.WVGA:
                        return new Uri("Assets/backWXGA.jpg", UriKind.Relative);
                    default:
                        throw new InvalidOperationException("Unknown resolution type");
                }
            }
        }

    }

}
