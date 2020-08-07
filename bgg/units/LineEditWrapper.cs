using System;
using System.ComponentModel;
using Godot;

public class LineEditWrapper<T> : Godot.Object where T: IFormattable {

    private T PrevValue;

    public LineEdit LineEdit { get; private set; }
    public T Value { get; private set; }
    public T DefaultValue { get; private set; }
    public String FormatString { get; set; }
    public Action<T> ValueChanged  { get; set; }

    public LineEditWrapper(LineEdit le, T defaultval, String format = null) {
        LineEdit = le;
        DefaultValue = defaultval;
        PrevValue = defaultval;
        FormatString = format;

	    LineEdit.Connect("text_entered", this, nameof(Edited));
	    LineEdit.Connect("focus_exited", this, nameof(Edited), new Godot.Collections.Array() { String.Empty });

        SetValue(DefaultValue);
    }

    private void Edited(String _) {
        var val = DefaultValue;
        var converter = TypeDescriptor.GetConverter(typeof(T));
        if (converter != null && converter.IsValid(LineEdit.Text))
            val = (T)converter.ConvertFromString(LineEdit.Text);
        SetValue(val);
    }

    public void SetValue(T v)
    {
        var prevText = String.IsNullOrWhiteSpace(FormatString) ? PrevValue.ToString() : PrevValue.ToString(FormatString, null);
        PrevValue = Value;
        Value = v;
        var newText = String.IsNullOrWhiteSpace(FormatString) ? Value.ToString() : Value.ToString(FormatString, null);
        LineEdit.Text = newText;
        if (prevText != newText)
        {
            ValueChanged?.Invoke(Value);
        }
    }
}