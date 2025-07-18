import axios from "axios";
import { z } from "zod";

export const WeatherForecastSchema = z.object({
    date: z.string(),
    temperatureC: z.number(),
    summary: z.string().nullable(),
    temperatureF: z.number(),
});
export const WeatherForecastArraySchema = z.array(WeatherForecastSchema);

export type WeatherForecast = z.infer<typeof WeatherForecastSchema>;

export const getWeather = async (): Promise<WeatherForecast[]> => {
    const { data } = await axios.get("api/weatherforecast");
    return WeatherForecastArraySchema.parse(data);
};