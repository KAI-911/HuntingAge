using System.Threading.Tasks;
using System;
public class RunOnce
{
    bool _flg = false;
    public bool Flg { get => _flg; set => _flg = value; }
    public void Run(System.Action action)
    {
        if (_flg) return;
        // �������s��
        action();
        // �^�U�l��^�ɐ؂�ւ���
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
