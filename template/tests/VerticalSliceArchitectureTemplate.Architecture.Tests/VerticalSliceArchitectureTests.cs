namespace VerticalSliceArchitectureTemplate.Architecture.Tests;

public class VerticalSliceArchitectureTests
{
    [Fact]
    public void Features_DependOn_Common() { }

    [Fact]
    public void Host_DependOn_Features() { }

    [Fact]
    public void Common_DependOn_Nothing() { }

    [Fact]
    public void Features_DontDependOn_EachOther() { }

    [Fact]
    public void UseCases_Are_CqrsNamed() { }

    [Fact]
    public void UseCases_Have_RequestDto() { }

    [Fact]
    public void UseCases_HaveResponseDto() { }

    [Fact]
    public void Domain_DependsOn_Nothing() { }

    [Fact]
    public void DomainEntity_Id_IsStrongId() { }

    [Fact]
    public void UseCases_Are_Sealed() { }

    [Fact]
    public void UseCases_AreInternal() { }

    [Fact]
    public void UseCases_Have_Endpoint() { }

    [Fact]
    public void UseCases_HaveChildClass_Request() { }

    [Fact]
    public void UseCases_HaveChildClass_Response() { }

    [Fact]
    public void UseCases_Implement_RequestHandler() { }

    [Fact]
    public void UseCasesRequest_RespondsWith_SiblingResponse() { }

    [Fact]
    public void UseCasesHandler_ImplementsFor_ChildDtos() { }
}
