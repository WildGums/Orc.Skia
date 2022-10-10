namespace Orc.Skia
{
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Matrix object extensions.
    /// </summary>
    public static class MatrixExtensions
    {
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
            var matrixTransform = new MatrixTransform
            {
                Matrix = matrix
            };

            var transformedRect = matrixTransform.TransformBounds(rectangle);
            return transformedRect;
        }
    }
}
