using System;

namespace RanNanDoh.ReadModels
{
    public interface IViewModel
    {
        Guid Id { get; }
    }

    public interface IViewModelWriter<in TViewModel>
    {
        void Update(Guid id, TViewModel viewModel);

        void Add(Guid id, TViewModel viewModel);

        void Drop();
    }

    public interface IViewModelReader<out TViewModel>
    {
        TViewModel Get(Guid id);
    }

    public interface IViewModelDropper
    {
        void Drop();
    }
}