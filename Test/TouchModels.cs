namespace Test;

public class TouchModels
{
    [Fact]
    public void Touch()
    {
        var actor = new Models.Actor();
        var a1 = actor.Id;
        var a2 = actor.Name;
        var cast = new Models.Cast();
        var c1 = cast.ActorId;
        var c2 = cast.Id;
        var c3 = cast.MovieId;
        var movie = new Models.Movie();
        var m1 = movie.Description;
        var m2 = movie.Id;
        var m3 = movie.Name;
        var sample = new Models.Sample();
        var s1 = sample.DateTime;
        var s2 = sample.Id;
        var s3 = sample.NullableDateTime;
        var s4 = sample.NullableGuid;
    }
}
