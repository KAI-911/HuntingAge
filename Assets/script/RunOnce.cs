
public class RunOnce
{
    bool flag = false;
    public void Run(System.Action action)
    {
        if (flag) return;
        // �������s��
        action();
        // �^�U�l��^�ɐ؂�ւ���
        flag = true;
    }
}
