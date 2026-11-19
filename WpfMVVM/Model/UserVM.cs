using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace WpfMVVM.Model;

public class UserVM : INotifyPropertyChanged
{
    public int Id { get; set; }

    private string _name = null!; // Ім'я
    private string _image = null!; // Зображення
    private string _phone = null!; // Номер телефону

    public string Name // Властивості для ім'я
    {
        get { return _name; }
        set
        {
            if (_name != value)
            {
                _name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }
    }

    public string Image // Властивості для зображення
    {
        get { return _image; }
        set
        {
            if (_image != value)
            {
                _image = value;
                NotifyPropertyChanged(nameof(Image));
            }
        }
    }

    public string Phone // Властивості для номеру телефону
    {
        get { return _phone; }
        set
        {
            if (_phone != value)
            {
                _phone = value;
                NotifyPropertyChanged(nameof(Phone));
            }
        }
    }

    // Оголошення події PropertyChangedEventHandler
    public event PropertyChangedEventHandler? PropertyChanged;

    // Запускає подію і передає інтерфейсу властивості
    public void NotifyPropertyChanged(string propName)
    {
        PropertyChanged?
            .Invoke(this, new PropertyChangedEventArgs(propName));
    }
}