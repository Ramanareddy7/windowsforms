//
// Copyright (c) Microsoft. All rights reserved.
// See https://aka.ms/csspeech/license201809 for the full license information.
//

#pragma once
#include <speechapi_c_common.h>

typedef enum { SpeechOutputFormat_Simple = 0, SpeechOutputFormat_Detailed = 1 } SpeechOutputFormat;

typedef enum
{
    // raw-8khz-8bit-mono-mulaw
    SpeechSynthesisOutputFormat_Raw8Khz8BitMonoMULaw = 1,

    // riff-16khz-16kbps-mono-siren
    // Unsupported by the service. Do not use this value.
    SpeechSynthesisOutputFormat_Riff16Khz16KbpsMonoSiren = 2,

    // audio-16khz-16kbps-mono-siren
    // Unsupported by the service. Do not use this value.
    SpeechSynthesisOutputFormat_Audio16Khz16KbpsMonoSiren = 3,

    // audio-16khz-32kbitrate-mono-mp3
    SpeechSynthesisOutputFormat_Audio16Khz32KBitRateMonoMp3 = 4,

    // audio-16khz-128kbitrate-mono-mp3
    SpeechSynthesisOutputFormat_Audio16Khz128KBitRateMonoMp3 = 5,

    // audio-16khz-64kbitrate-mono-mp3
    SpeechSynthesisOutputFormat_Audio16Khz64KBitRateMonoMp3 = 6,

    // audio-24khz-48kbitrate-mono-mp3
    SpeechSynthesisOutputFormat_Audio24Khz48KBitRateMonoMp3 = 7,

    // audio-24khz-96kbitrate-mono-mp3
    SpeechSynthesisOutputFormat_Audio24Khz96KBitRateMonoMp3 = 8,

    // audio-24khz-160kbitrate-mono-mp3
    SpeechSynthesisOutputFormat_Audio24Khz160KBitRateMonoMp3 = 9,

    // raw-16khz-16bit-mono-truesilk
    SpeechSynthesisOutputFormat_Raw16Khz16BitMonoTrueSilk = 10,

    // riff-16khz-16bit-mono-pcm
    SpeechSynthesisOutputFormat_Riff16Khz16BitMonoPcm = 11,

    // riff-8khz-16bit-mono-pcm
    SpeechSynthesisOutputFormat_Riff8Khz16BitMonoPcm = 12,

    // riff-24khz-16bit-mono-pcm
    SpeechSynthesisOutputFormat_Riff24Khz16BitMonoPcm = 13,

    // riff-8khz-8bit-mono-mulaw
    SpeechSynthesisOutputFormat_Riff8Khz8BitMonoMULaw = 14,

    // raw-16khz-16bit-mono-pcm
    SpeechSynthesisOutputFormat_Raw16Khz16BitMonoPcm = 15,

    // raw-24khz-16bit-mono-pcm
    SpeechSynthesisOutputFormat_Raw24Khz16BitMonoPcm = 16,

    // raw-8khz-16bit-mono-pcm
    SpeechSynthesisOutputFormat_Raw8Khz16BitMonoPcm = 17,

    // ogg-16khz-16bit-mono-opus
    SpeechSynthesisOutputFormat_Ogg16khz16BitMonoOpus = 18,

    // ogg-24khz-24bit-mono-opus
    SpeechSynthesisOutputFormat_Ogg24Khz16BitMonoOpus = 19,

    // raw-48khz-16bit-mono-pcm
    SpeechSynthesisOutputFormat_Raw48Khz16BitMonoPcm = 20,

    // riff-48khz-16bit-mono-pcm
    SpeechSynthesisOutputFormat_Riff48Khz16BitMonoPcm = 21,

    // audio-48khz-96kbitrate-mono-mp3
    SpeechSynthesisOutputFormat_Audio48Khz96KBitRateMonoMp3 = 22,

    // audio-48khz-192kbitrate-mono-mp3
    SpeechSynthesisOutputFormat_Audio48Khz192KBitRateMonoMp3 = 23,

    // ogg-48khz-16bit-mono-opus
    SpeechSynthesisOutputFormat_Ogg48Khz16BitMonoOpus = 24,

    // webm-16khz-16bit-mono-opus
    SpeechSynthesisOutputFormat_Webm16Khz16BitMonoOpus = 25,

    // webm-24khz-16bit-mono-opus
    SpeechSynthesisOutputFormat_Webm24Khz16BitMonoOpus = 26,

    // raw-24khz-16bit-mono-truesilk
    SpeechSynthesisOutputFormat_Raw24Khz16BitMonoTrueSilk = 27,

    /// raw-8khz-8bit-mono-alaw
    SpeechSynthesisOutputFormat_Raw8Khz8BitMonoALaw = 28,

    /// riff-8khz-8bit-mono-alaw
    SpeechSynthesisOutputFormat_Riff8Khz8BitMonoALaw = 29,
} Speech_Synthesis_Output_Format;

typedef enum
{
    // Using URI query parameter to pass property settings to service.
    SpeechConfig_ServicePropertyChannel_UriQueryParameter = 0,

    // Using HttpHeader to set a key/value in a HTTP header.
    SpeechConfig_ServicePropertyChannel_HttpHeader = 1
} SpeechConfig_ServicePropertyChannel;

typedef enum
{
    SpeechConfig_ProfanityMasked = 0,
    SpeechConfig_ProfanityRemoved = 1,
    SpeechConfig_ProfanityRaw = 2
} SpeechConfig_ProfanityOption;

SPXAPI_(bool) speech_config_is_handle_valid(SPXSPEECHCONFIGHANDLE hconfig);
SPXAPI speech_config_from_subscription(SPXSPEECHCONFIGHANDLE* hconfig, const char* subscription, const char* region);
SPXAPI speech_config_from_authorization_token(SPXSPEECHCONFIGHANDLE* hconfig, const char* authToken, const char* region);
SPXAPI speech_config_from_endpoint(SPXSPEECHCONFIGHANDLE * hconfig, const char* endpoint, const char* subscription);
SPXAPI speech_config_from_host(SPXSPEECHCONFIGHANDLE* hconfig, const char* host, const char* subscription);
SPXAPI speech_config_release(SPXSPEECHCONFIGHANDLE hconfig);
SPXAPI speech_config_get_property_bag(SPXSPEECHCONFIGHANDLE hconfig, SPXPROPERTYBAGHANDLE* hpropbag);
SPXAPI speech_config_set_audio_output_format(SPXSPEECHCONFIGHANDLE hconfig, Speech_Synthesis_Output_Format formatId);
SPXAPI speech_config_set_service_property(SPXSPEECHCONFIGHANDLE configHandle, const char* propertyName, const char* propertyValue, SpeechConfig_ServicePropertyChannel channel);
SPXAPI speech_config_set_profanity(SPXSPEECHCONFIGHANDLE configHandle, SpeechConfig_ProfanityOption profanity);

