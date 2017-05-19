// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MatrixExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Skia
{
#if NETFX_CORE
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml.Media;
#else
    using System.Windows;
    using System.Windows.Media;
#endif

    /// <summary>
    /// Matrix object extensions.
    /// </summary>
    public static class MatrixExtensions
    {
        #region Methods
        public static Matrix Copy(this Matrix matrix)
        {
            return new Matrix(matrix.M11,
                matrix.M12,
                matrix.M21,
                matrix.M22,
                matrix.OffsetX,
                matrix.OffsetY);
        }

        public static Matrix Multiply(this Matrix matrix, Matrix secondMatrix)
        {
            //return matrix * secondMatrix;

            var finalMatrix = new Matrix(matrix.M11 * secondMatrix.M11,
                matrix.M12 * secondMatrix.M12,
                matrix.M21 * secondMatrix.M21,
                matrix.M22 * secondMatrix.M22,
                matrix.OffsetX * secondMatrix.OffsetX,
                matrix.OffsetY * secondMatrix.OffsetY);

            return finalMatrix;
        }

        public static Rect TransformRectangle(this Matrix matrix, Rect rectangle)
        {
            //rectangle.Transform(matrix);
            //return rectangle;

            var matrixTransform = new MatrixTransform
            {
                Matrix = matrix
            };

            var transformedRect = matrixTransform.TransformBounds(rectangle);
            return transformedRect;
        }
        #endregion
    }
}