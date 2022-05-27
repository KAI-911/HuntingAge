
public class RunOnce
{
    bool flag = false;
    public void Run(System.Action action)
    {
        if (flag) return;
        // 処理を行う
        action();
        // 真偽値を真に切り替える
        flag = true;
    }
}
