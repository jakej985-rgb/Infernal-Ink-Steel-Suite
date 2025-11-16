#pragma once

#include <QLoggingCategory>
#include <QtGlobal>

inline bool isFacebookDebugLoggingEnabled(const QLoggingCategory &category)
{
#ifdef QT_DEBUG
    Q_UNUSED(category);
    return true;
#else
    static const bool envEnabled = qEnvironmentVariableIsSet("TSM_FACEBOOK_DEBUG");
    return envEnabled || category.isDebugEnabled();
#endif
}

