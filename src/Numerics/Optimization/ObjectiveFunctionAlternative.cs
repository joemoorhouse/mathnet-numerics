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
}
