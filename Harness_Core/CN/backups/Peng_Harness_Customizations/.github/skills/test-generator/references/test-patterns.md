## Common Test Patterns

### Constructor / Initialization Tests

```csharp
[Fact]
public void Constructor_WithValidParameters_ShouldInitializeProperties()
{
    // Arrange & Act
    var sut = new ClassName(param1, param2);

    // Assert
    sut.Property1.Should().Be(param1);
    sut.Property2.Should().Be(param2);
}

[Fact]
public void Constructor_WithNullParameter_ShouldThrowArgumentNullException()
{
    // Act
    var act = () => new ClassName(null);

    // Assert
    act.Should().Throw<ArgumentNullException>()
       .WithParameterName("paramName");
}
```

### Property Setter Tests

```csharp
[Fact]
public void Property_WhenSet_ShouldRaisePropertyChanged()
{
    // Arrange
    var sut = new ViewModelClass();
    using var monitor = sut.Monitor();

    // Act
    sut.SomeProperty = newValue;

    // Assert
    monitor.Should().RaisePropertyChangedFor(x => x.SomeProperty);
}
```

### Command / Method Tests

```csharp
[Theory]
[InlineData(1, 4096)]    // min boundary
[InlineData(2048, 4096)] // mid range
[InlineData(4096, 4096)] // max boundary
public void SetWindowWidth_WhenValueInRange_ShouldAccept(int input, int maxAllowed)
{
    // Arrange
    var sut = CreateSut();

    // Act
    sut.SetWindowWidth(input);

    // Assert
    sut.WindowWidth.Should().Be(input);
}

[Theory]
[InlineData(0)]
[InlineData(-1)]
[InlineData(4097)]
public void SetWindowWidth_WhenValueOutOfRange_ShouldThrow(int invalidValue)
{
    // Arrange
    var sut = CreateSut();

    // Act
    var act = () => sut.SetWindowWidth(invalidValue);

    // Assert
    act.Should().Throw<ArgumentOutOfRangeException>();
}
```

### Async Method Tests

```csharp
[Fact]
public async Task LoadImageAsync_WhenFileExists_ShouldReturnImageData()
{
    // Arrange
    var mockFileService = new Mock<IFileService>();
    mockFileService.Setup(f => f.ReadAsync(filePath))
                   .ReturnsAsync(expectedBytes);
    var sut = new ImageLoader(mockFileService.Object);

    // Act
    var result = await sut.LoadImageAsync(filePath);

    // Assert
    result.Should().NotBeNull();
    result.Data.Should().BeEquivalentTo(expectedBytes);
}
```

### Event Tests

```csharp
[Fact]
public void Process_WhenComplete_ShouldRaiseCompletedEvent()
{
    // Arrange
    var sut = new Processor();
    var eventRaised = false;
    sut.Completed += (s, e) => eventRaised = true;

    // Act
    sut.Process();

    // Assert
    eventRaised.Should().BeTrue();
}
```

### IDisposable Tests

```csharp
[Fact]
public void Dispose_ShouldReleaseResources()
{
    // Arrange
    var mockResource = new Mock<IDisposable>();
    var sut = new ResourceHolder(mockResource.Object);

    // Act
    sut.Dispose();

    // Assert
    mockResource.Verify(r => r.Dispose(), Times.Once);
}
```

## WPF / MVVM Specific Patterns

### ViewModel Command Tests

```csharp
[Fact]
public void SaveCommand_WhenDataValid_ShouldCallRepository()
{
    // Arrange
    var mockRepo = new Mock<IRepository>();
    var sut = new EditViewModel(mockRepo.Object) { Name = "Test" };

    // Act
    sut.SaveCommand.Execute(null);

    // Assert
    mockRepo.Verify(r => r.Save(It.IsAny<Entity>()), Times.Once);
}

[Fact]
public void SaveCommand_WhenDataInvalid_CanExecuteShouldReturnFalse()
{
    // Arrange
    var sut = new EditViewModel(Mock.Of<IRepository>()) { Name = "" };

    // Act
    var canExecute = sut.SaveCommand.CanExecute(null);

    // Assert
    canExecute.Should().BeFalse();
}
```
