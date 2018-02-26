﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using NodeNetwork.Utilities;
using NodeNetwork.ViewModels;
using NodeNetwork.Views;
using ReactiveUI;

namespace NodeNetwork.Toolkit.ValueNode
{
    /// <summary>
    /// A node input that keeps a list of the latest values produced by all of the connected ValueNodeOutputViewModels.
    /// This input can take multiple connections, ValueNodeInputViewModel cannot.
    /// </summary>
    /// <typeparam name="T">The type of object this input can receive</typeparam>
    public class ValueListNodeInputViewModel<T> : NodeInputViewModel
    {
        static ValueListNodeInputViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeInputView(), typeof(IViewFor<ValueListNodeInputViewModel<T>>));
        }

        /// <summary>
        /// The current values of the outputs connected to this input
        /// </summary>
        public IReadOnlyReactiveList<T> Values { get; } = new ReactiveList<T>();
        
        public ValueListNodeInputViewModel()
        {
            MaxConnections = Int32.MaxValue;
            ConnectionValidator = new ValueListConnectionValidator<T>();

            Connections.Changed.SelectMany(change =>
            {
                return Connections.Select(c => ((ValueNodeOutputViewModel<T>) c.Output).Value).CombineLatest();
            }).BindListContents(this, vm => vm.Values);
        }
    }

    public class ValueListConnectionValidator<T> : ConnectionValidator
    {
        public override ConnectionValidationResult Validate(PendingConnectionViewModel pending)
        {
            return new ConnectionValidationResult(pending.Output is ValueNodeOutputViewModel<T>, null);
        }
    }
}
