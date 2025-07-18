import { useQuery } from "@tanstack/react-query";
import { getWeather } from "@/api/weather";
import type { WeatherForecast } from "@/api/weather";
import { Button } from "@/components/ui/button";

export default function WeatherDemo() {
    const { data, isLoading, error, refetch } = useQuery<WeatherForecast[]>({
        queryKey: ["weather"],
        queryFn: getWeather,
    });

    return (
        <div className="min-h-screen bg-gray-50 flex flex-col items-center justify-center">
            <div className="bg-white p-8 rounded shadow w-full max-w-md">
                <h1 className="text-2xl font-bold mb-4">Weather Forecast</h1>
                <Button onClick={() => refetch()} className="mb-4">Refresh</Button>
                {isLoading && <div>Loading...</div>}
                {error && <div className="text-red-500">Error loading data</div>}
                {data && (
                    <ul>
                        {data.map((item) => (
                            <li key={item.date} className="mb-2 flex justify-between">
                                <span>{item.date}</span>
                                <span>{item.summary}</span>
                                <span>{item.temperatureC}°C</span>
                            </li>
                        ))}
                    </ul>
                )}
            </div>
        </div>
    );
}