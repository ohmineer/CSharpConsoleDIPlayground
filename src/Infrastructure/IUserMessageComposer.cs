namespace ConsoleDIPlayground;

public interface IUserMessageComposer
{
  UserMessage Compose(params object[] p);
}
