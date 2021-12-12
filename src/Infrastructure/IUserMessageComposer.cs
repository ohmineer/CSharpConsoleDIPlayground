namespace ConsoleDIPlayground.Infrastructure;

public interface IUserMessageComposer
{
  UserMessage Compose(params object[] p);
}
