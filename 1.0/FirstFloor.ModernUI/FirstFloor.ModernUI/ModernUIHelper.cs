using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FirstFloor.ModernUI
{
    /// <summary>
    /// Provides various common helper methods.
    /// </summary>
    public static class ModernUIHelper
    {
        private static bool? isInDesignMode;

        /// <summary>
        /// Determines whether the current code is executed in a design time environment such as Visual Studio or Blend.
        /// </summary>
        public static bool IsInDesignMode
        {
            get
            {
                if (!isInDesignMode.HasValue) {
                    isInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
                }
                return isInDesignMode.Value;
            }
        }
    }
}
