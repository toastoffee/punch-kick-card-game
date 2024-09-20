namespace ToffeeFactory {
  public struct Ingredient {
    public string name;
    public int count;

    private static Ingredient Empty = new Ingredient() {
      name = "empty",
      count = 0,
    };
  }
}