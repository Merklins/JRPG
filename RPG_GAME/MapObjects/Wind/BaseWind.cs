using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace RPG_GAME;

public class BaseWind
{
    public float OscillationSpeed { get; set; }
    public float OscillationSpeedConst { get; set; }
    public float Amplitude { get; set; }
    public float AmplitudeConst { get; set; }
    public float PhaseShift { get; set; }
    public float DampingCoefficient { get; set; }
    
    public float windOffset = 0f;
    public float time = 0f;
    public float timeToRandomChangeParametersConst;
    public float timeToRandomChangeParameters = 0f;
    public float lastWindOffset = 0f;

    public BaseWind(float oscillationSpeed, float amplitude, float phaseShift,
        float dampingCoefficient, float timeToRandomChangeParameters)
    {
        this.OscillationSpeed = oscillationSpeed;
        this.OscillationSpeedConst = oscillationSpeed;
        this.Amplitude = amplitude;
        this.AmplitudeConst = amplitude;
        this.PhaseShift = phaseShift;
        this.DampingCoefficient = dampingCoefficient;
        this.timeToRandomChangeParametersConst = timeToRandomChangeParameters;
    }

    public virtual void Update(GameTime gameTime)
    {
        if (timeToRandomChangeParametersConst != 0f)
            timeToRandomChangeParameters += (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        time += (float)gameTime.ElapsedGameTime.TotalSeconds * OscillationSpeed;

        // В начале вычисляем сырое смещение ветра без учета замедления
        float rawWindOffset = Amplitude * (float)Math.Sin(time + PhaseShift);

        // Расчет коэффициента замедления отклонения относительно близости текущего смещения к максимальной амплитуде
        float damping = 1f - (Math.Abs(rawWindOffset) / Amplitude) * DampingCoefficient;
        damping = Math.Clamp(damping, 0f, 1f); // убедимся, что замедление не выходит за пределы [0, 1]

        // Применяем замедление к смещению ветра
        windOffset = rawWindOffset * damping;

        // объект примерно выравнялся, чтобы при смене амплитуды не было резкого скачка
        if ( !(-0.1f <= windOffset && windOffset <= 0.1f) ) return;
        
        if ( !(timeToRandomChangeParameters >= timeToRandomChangeParametersConst
               && timeToRandomChangeParametersConst != 0) ) return;
            
        float multiplyAmplitude = Random.Shared.NextFloat(0.75f, 1.5f);
        float multiplyOscillationSpeed = Random.Shared.NextFloat(0.75f, 1.5f);
        Amplitude = AmplitudeConst * multiplyAmplitude;
        OscillationSpeed = OscillationSpeedConst * multiplyOscillationSpeed;
        DampingCoefficient = Random.Shared.NextFloat(0f, 1f);
        timeToRandomChangeParameters = 0f;
    }
}