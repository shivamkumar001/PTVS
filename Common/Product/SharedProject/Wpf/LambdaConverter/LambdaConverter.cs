﻿// Visual Studio Shared Project
// Copyright(c) Microsoft Corporation
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABLITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.VisualStudioTools.Wpf {
    public class LambdaConverter : IValueConverter, IMultiValueConverter {
        private readonly Func<object, object> lambda;
        private readonly Func<object[], object> multiLambda;

        private LambdaConverter(Func<object, object> lambda) {
            this.lambda = lambda;
            this.multiLambda = (args) => {
                Debug.Assert(args.Length == 1);
                return lambda(args[0]);
            };
        }

        private LambdaConverter(Func<object[], object> multiLambda) {
            this.multiLambda = multiLambda;
            this.lambda = (arg) => multiLambda(new[] { arg });
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return lambda(value);
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            return multiLambda(values);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        public static LambdaConverter Create(Func<dynamic, object> lambda) {
            return Create<dynamic>(lambda);
        }

        public static LambdaConverter Create<T1>(Func<T1, object> lambda) {
            return new LambdaConverter(arg => {
                    if (arg == DependencyProperty.UnsetValue) {
                        return DependencyProperty.UnsetValue;
                    }

                    return lambda((T1)arg);
                });
        }

        public static LambdaConverter Create(Func<dynamic, dynamic, object> lambda) {
            return Create<dynamic, dynamic>(lambda);
        }

        public static LambdaConverter Create<T1, T2>(Func<T1, T2, object> lambda) {
            return new LambdaConverter(
                (args) => {
                    Debug.Assert(args.Length == 2);

                    if (args[0] == DependencyProperty.UnsetValue) {
                        return DependencyProperty.UnsetValue;
                    }
                    if (args[1] == DependencyProperty.UnsetValue) {
                        return DependencyProperty.UnsetValue;
                    }

                    return lambda((T1)args[0], (T2)args[1]);
                });
        }

        public static LambdaConverter Create(Func<dynamic, dynamic, dynamic, object> lambda) {
            return Create<dynamic, dynamic, dynamic>(lambda);
        }

        public static LambdaConverter Create<T1, T2, T3>(Func<T1, T2, T3, object> lambda) {
            return new LambdaConverter(
                (args) => {
                    Debug.Assert(args.Length == 3);

                    if (args[0] == DependencyProperty.UnsetValue) {
                        return DependencyProperty.UnsetValue;
                    }
                    if (args[1] == DependencyProperty.UnsetValue) {
                        return DependencyProperty.UnsetValue;
                    }
                    if (args[2] == DependencyProperty.UnsetValue) {
                        return DependencyProperty.UnsetValue;
                    }

                    return lambda((T1)args[0], (T2)args[1], (T3)args[2]);
                });
        }

        public static LambdaConverter CreateMulti<T>(Func<T[], object> lambda) => new LambdaConverter(args => {
            if (args.Any(t => t == DependencyProperty.UnsetValue)) {
                return DependencyProperty.UnsetValue;
            }

            return lambda(args.Cast<T>().ToArray());
        });
    }
}
