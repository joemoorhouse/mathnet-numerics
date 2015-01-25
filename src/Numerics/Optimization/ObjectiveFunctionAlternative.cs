using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra;

namespace MathNet.Numerics.OptimizationAlternative
{
    /// <summary>
    /// A version that tries to combine existing IObjectiveFunction and IEvaluation
    /// </summary>
    public interface IObjectiveFunction
    {
        Vector<double> Point { get; set; }
        double Value { get; }
        Vector<double> Gradient { get; }
        Matrix<double> Hessian { get; }
    }

    [Flags]
    public enum EvaluationStatus { None = 0, Value = 1, Gradient = 2, Hessian = 4 }

    public delegate double Function(Vector<double> point);

    public delegate Vector<double> Gradient(Vector<double> point);

    public delegate Matrix<double> Hessian(Vector<double> point);

    public class ObjectiveFunction : IObjectiveFunction
    {
        public ObjectiveFunction(Function function, Gradient gradient, Hessian hessian)
        {
            _function = function;
            _gradient = gradient;
            _hessian = hessian;
        }

        public Vector<double> Point
        {
            get
            {
                return _point;
            }
            set
            {
                _point = value;
                _evaluationStatus = EvaluationStatus.None;
            }
        }

        public double Value
        {
            get
            {
                if (!_evaluationStatus.HasFlag(EvaluationStatus.Value))
                {
                    _functionValue = _function(_point);
                    _evaluationStatus |= EvaluationStatus.Value;
                }
                return _functionValue;
            }
        }

        public Vector<double> Gradient
        {
            get
            {
                if (!_evaluationStatus.HasFlag(EvaluationStatus.Gradient))
                {
                    _gradientValue = _gradient(_point);
                    _evaluationStatus |= EvaluationStatus.Gradient;
                }
                return _gradientValue;
            }
        }


        public Matrix<double> Hessian
        {
            get
            {
                if (!_evaluationStatus.HasFlag(EvaluationStatus.Gradient))
                {
                    _hessianValue = _hessian(_point);
                    _evaluationStatus |= EvaluationStatus.Gradient;
                }
                return _hessianValue;
            }
        }

        public bool GradientSupported { get { return _gradient != null; } }

        #region Objective Function Delegates

        private Function _function;
        private Gradient _gradient;
        private Hessian _hessian;

        #endregion

        #region Objective Function Evaluation Storage

        private Vector<double> _point;
        private double _functionValue;
        private Vector<double> _gradientValue;
        private Matrix<double> _hessianValue;
        
        private EvaluationStatus _evaluationStatus;

        #endregion
    }

    public class Example
    {
        public void SetUp()
        {
            var objectiveFunction = new ObjectiveFunction(
                RosenbrockFunction.Value, RosenbrockFunction.Gradient, RosenbrockFunction.Hessian);
        }
    }

    public static class RosenbrockFunction
    {
        public static double Value(Vector<double> input)
        {
            return Math.Pow((1 - input[0]), 2) + 100 * Math.Pow((input[1] - input[0] * input[0]), 2);
        }

        public static Vector<double> Gradient(Vector<double> input)
        {
            Vector<double> output = new MathNet.Numerics.LinearAlgebra.Double.DenseVector(2);
            output[0] = -2 * (1 - input[0]) + 200 * (input[1] - input[0] * input[0]) * (-2 * input[0]);
            output[1] = 2 * 100 * (input[1] - input[0] * input[0]);
            return output;
        }

        public static Matrix<double> Hessian(Vector<double> input)
        {

            Matrix<double> output = new MathNet.Numerics.LinearAlgebra.Double.DenseMatrix(2, 2);
            output[0, 0] = 2 - 400 * input[1] + 1200 * input[0] * input[0];
            output[1, 1] = 200;
            output[0, 1] = -400 * input[0];
            output[1, 0] = output[0, 1];
            return output;
        }
    }
}
