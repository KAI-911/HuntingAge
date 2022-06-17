using System.Threading.Tasks;
using System;
public class RunOnce
{
    bool _flg = false;
    public bool Flg { get => _flg; set => _flg = value; }
    public void Run(System.Action action)
    {
        if (_flg) return;
        // 処理を行う
        action();
        // 真偽値を真に切り替える
        _flg = true;
    }
    public async Task WaitForAsync(float seconds, Action action)
    {
        if (_flg) return;
        _flg = true;
        await Task.Delay(TimeSpan.FromSeconds(seconds));
        action();
    }

}
