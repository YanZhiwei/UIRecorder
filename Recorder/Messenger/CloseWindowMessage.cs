using System;

namespace Recorder.Messenger;

internal sealed class CloseWindowMessage
{
    public WeakReference? Sender { get; set; }
}