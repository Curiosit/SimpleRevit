﻿using System.Windows;

namespace SimpleRevit;

/// <summary>
/// A base command class for MVVM design.
/// </summary>
/// <typeparam name="TView">your view.</typeparam>
/// <typeparam name="TViewModel">your view model.</typeparam>
public abstract class CmdBaseMvvm<TView, TViewModel> : CmdBase
    where TView : Window 
    where TViewModel : SimpleRevitViewModel
{
    /// <summary>
    /// The window.
    /// </summary>
    public TView View { get; private set; }

    /// <summary>
    /// The view model.
    /// </summary>
    public TViewModel ViewModel { get; private set; }

    DateTime _startTime;

    internal override void PreExecute()
    {
        var thread = new Thread(() =>
        {
            View = Activator.CreateInstance<TView>();
            View.DataContext = ViewModel = Activator.CreateInstance<TViewModel>();
            View.ShowDialog();
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        _startTime = DateTime.Now;

        base.PreExecute();
    }

    internal override void PostExecute()
    {
        while (View == null)
        {
            Task.Delay(100).Wait();
        }

        View.Dispatcher.Invoke(View.Close);

        base.PostExecute();
    }

    /// <summary>
    /// Update your calculating percent while running.
    /// </summary>
    /// <param name="percent"></param>
    public void UpdatePercent(double percent)
    {
        if(ViewModel == null) return;
        if(percent == 0)
        {
            _startTime = DateTime.Now;
        }
        ViewModel.Percent = percent;
        ViewModel.ElapsedTime = DateTime.Now - _startTime;
    }
}