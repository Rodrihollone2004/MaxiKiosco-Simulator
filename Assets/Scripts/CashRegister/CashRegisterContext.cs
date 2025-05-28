public class CashRegisterContext 
{
    public static NPC_Controller CurrentClientController { get; private set; }

    public static void SetCurrentClient(NPC_Controller controller)
    {
        CurrentClientController = controller;
    }

    public static bool IsClientInCashRegister()
    {
        return CurrentClientController != null && CurrentClientController.isInCashRegister;
    }
}
