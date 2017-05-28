using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;
using System.Diagnostics;

namespace TarkOrm.Tests
{
    [TestClass]
    public class LambdaExpressionTests
    {
        [TestMethod]
        public void LambdaExpressionTests1()
        {
            var br = new Country()
            {
                Name = "Brazil"
            };

            var w = Test2(br, a => a.Name.Length > 0 && a.Name.Length < 10 || a.Name.Length == 50);

            var x = Test(br, a => a.Name.Length);

            var y = Test(br, a => a.Name.Length > 7 && a.CountryID <= 2 && a.CountryID > 1 && a.CountryID  == 2);

            var lista = new List<Country>();                        
        }
        
        public bool Test2<TSource>(TSource source, Expression<Func<TSource, bool>> propertyLambda)
        {
            Type type = typeof(TSource);
            MemberExpression member = propertyLambda.Body as MemberExpression;

            return false;
        }
        
        public bool Test<TSource, TProperty> (TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);
            MemberExpression member = propertyLambda.Body as MemberExpression;
                    
            return false;
        }

        //public static RouteValueDictionary GetInfo<T, P>(this HtmlHelper html, Expression<Func<T, P>> action) where T : class
        //{
        //    var expression = (MemberExpression)action.Body;
        //    string name = expression.Member.Name;

        //    return GetInfo(html, name);
        //}

        #region "GetPropertyInfo"
        public PropertyInfo GetPropertyInfo<TSource, TProperty>(
            TSource source,
            Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }

        public PropertyInfo GetPropertyFromExpression<T>(Expression<Func<T, object>> GetPropertyLambda)
        {
            MemberExpression Exp = null;

            //this line is necessary, because sometimes the expression comes in as Convert(originalexpression)
            if (GetPropertyLambda.Body is UnaryExpression)
            {
                var UnExp = (UnaryExpression)GetPropertyLambda.Body;
                if (UnExp.Operand is MemberExpression)
                {
                    Exp = (MemberExpression)UnExp.Operand;
                }
                else
                    throw new ArgumentException();
            }
            else if (GetPropertyLambda.Body is MemberExpression)
            {
                Exp = (MemberExpression)GetPropertyLambda.Body;
            }
            else
            {
                throw new ArgumentException();
            }

            return (PropertyInfo)Exp.Member;
        }

        public string GetName<TSource, TField>(Expression<Func<TSource, TField>> Field)
        {
            return (Field.Body as MemberExpression ?? ((UnaryExpression)Field.Body).Operand as MemberExpression).Member.Name;
        }

        public string GetNameX<TSource, TField>(Expression<Func<TSource, TField>> Field)
        {
            if (object.Equals(Field, null))
            {
                throw new NullReferenceException("Field is required");
            }

            MemberExpression expr = null;

            if (Field.Body is MemberExpression)
            {
                expr = (MemberExpression)Field.Body;
            }
            else if (Field.Body is UnaryExpression)
            {
                expr = (MemberExpression)((UnaryExpression)Field.Body).Operand;
            }
            else
            {
                const string Format = "Expression '{0}' not supported.";
                string message = string.Format(Format, Field);

                throw new ArgumentException(message, "Field");
            }

            return expr.Member.Name;
        }
        #endregion

        [TestMethod]
        public void TestLamba1()
        {
            ParameterExpression value = Expression.Parameter(typeof(int), "value");
            ParameterExpression result = Expression.Parameter(typeof(bool), "result");
            LabelTarget label = Expression.Label(typeof(int));

            //Expression.
            BlockExpression blockExprMaioridade = Expression.Block(
                new[] { result },
                Expression.IfThenElse(
                    Expression.GreaterThan(value, Expression.Constant(17)),
                    Expression.Assign(result, Expression.Constant(true)),
                    Expression.Assign(result, Expression.Constant(false))
                ), result
            );

            var lambda = Expression.Lambda<Func<int, bool>>(blockExprMaioridade, value).Compile();
            var lambdaResult = lambda(5);
            var lambdaResult2 = lambda(20);
        }

        [TestMethod]
        public void TestLamba2()
        {
            ParameterExpression value = Expression.Parameter(typeof(Pessoa), "value");
            ParameterExpression result = Expression.Parameter(typeof(bool), "result");
            LabelTarget label = Expression.Label(typeof(int));

            var pessoa = new Pessoa() { Id = 1, Nome = "TARCISIO", Idade = 28 };
            var pessoa2 = new Pessoa() { Id = 2, Nome = "MARIA", Idade = 17 };
            var pessoa3 = new Pessoa() { Id = 3, Nome = "Joao", Idade = 23 };

            //Expression
            BlockExpression blockExprMaioridade = Expression.Block(
                new[] { result },
                Expression.IfThenElse(
                    Expression.Equal(Expression.PropertyOrField(value, "Nome"), Expression.Constant("TARCISIO")),
                    Expression.Assign(result, Expression.Constant(true)),
                    Expression.Assign(result, Expression.Constant(false))
                ), result
            );

            var lambda = Expression.Lambda<Func<Pessoa, bool>>(blockExprMaioridade, value).Compile();
            var lambdaResult = lambda(pessoa);
            var lambdaResult2 = lambda(pessoa2);
        }

        [TestMethod]
        public void TestLamba3()
        {
            var listaRegras = new List<Regra>() {
                new Regra() { Field = "Nome", Operator = "=", Valor = "MARIA" },
                new Regra() { Field = "Idade", Operator = "<", Valor = 18 }
            };

            var listaRegrasCompiladas = listaRegras.Select(x =>
                new ConditionalExpressionFactory().Build<Pessoa>(x.Operator, x.Field, x.Valor)
            );

            var pessoa = new Pessoa() { Id = 1, Nome = "TARCISIO", Idade = 28 };
            var pessoa2 = new Pessoa() { Id = 2, Nome = "MARIA", Idade = 17 };
            var pessoa3 = new Pessoa() { Id = 3, Nome = "Joao", Idade = 23 };

            var result = AtendeAsRegras<Pessoa>(listaRegrasCompiladas, pessoa2);

        }

        private bool AtendeAsRegras<T>(IEnumerable<Func<T, bool>> regras, T item)
        {
            foreach (var regra in regras)
            {
                if (!regra(item))
                    return false;
            }
            return true;
        }

        [TestMethod]
        public void TestLambaBenchmark()
        {
            var listaRegras = new List<Regra>() {
                new Regra() { Field = "Nome", Operator = "=", Valor = "TARCISIO" },
                new Regra() { Field = "Idade", Operator = ">", Valor = 18 }
            };

            var listaRegrasCompiladas = listaRegras.Select(x =>
                new ConditionalExpressionFactory().Build<Pessoa>(x.Operator, x.Field, x.Valor)
            );

            var pessoa = new Pessoa() { Id = 1, Nome = "TARCISIO", Idade = 28 };
            var pessoa2 = new Pessoa() { Id = 2, Nome = "MARIA", Idade = 17 };
            var pessoa3 = new Pessoa() { Id = 3, Nome = "Joao", Idade = 23 };

            Stopwatch watch = new Stopwatch();
            watch.Start();

            for (int i = 0; i < 10000; i++)
            {
                foreach (var item in listaRegrasCompiladas)
                {
                    var result = item(pessoa);
                }
                //for (int j = 0; j < listaRegrasCompiladas.Count(); j++)
                //{
                //    var result = listaRegrasCompiladas.ElementAt(j)(pessoa);
                //}                
            }

            watch.Stop();
            Debug.Write($"Watch Result { watch.ElapsedMilliseconds }");
        }

        public class Pessoa
        {
            public int Id { get; set; }

            public string Nome { get; set; }

            public int Idade { get; set; }
        }

        public class Regra
        {
            public string Field { get; set; }

            public string Operator { get; set; }

            public object Valor { get; set; }
        }

        public class RegraExpression
        {
            public Expression Field { get; set; }

            public ConditionalExpression Operator { get; set; }

            public Expression Valor { get; set; }
        }

        public class ConditionalExpressionFactory
        {
            public Func<T, bool> Build<T>(string operation, string fieldname, object value)
            {
                ParameterExpression objParameter = Expression.Parameter(typeof(T), "objParameter");
                ParameterExpression result = Expression.Parameter(typeof(bool), "result");
                BinaryExpression binExpression;

                switch (operation)
                {
                    case "=":
                        binExpression = Expression.Equal(Expression.PropertyOrField(objParameter, fieldname), Expression.Constant(value));
                        break;
                    case ">":
                        binExpression = Expression.GreaterThan(Expression.PropertyOrField(objParameter, fieldname), Expression.Constant(value));
                        break;
                    case "<":
                        binExpression = Expression.LessThan(Expression.PropertyOrField(objParameter, fieldname), Expression.Constant(value));
                        break;
                    case ">=":
                        binExpression = Expression.GreaterThanOrEqual(Expression.PropertyOrField(objParameter, fieldname), Expression.Constant(value));
                        break;
                    case "<=":
                        binExpression = Expression.LessThanOrEqual(Expression.PropertyOrField(objParameter, fieldname), Expression.Constant(value));
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                var finalExpression = Expression.Block(
                    new[] { result },
                    Expression.IfThenElse(
                        binExpression,
                        Expression.Assign(result, Expression.Constant(true)),
                        Expression.Assign(result, Expression.Constant(false))
                    ), result
                );

                return Expression.Lambda<Func<T, bool>>(finalExpression, objParameter).Compile();
            }
        }

    }
}
