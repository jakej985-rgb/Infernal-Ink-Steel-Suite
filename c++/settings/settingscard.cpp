#include "settingscard.h"
#include "helper/thememanager.h"
#include "themes/theme.h"

#include <QGraphicsDropShadowEffect>
#include <QPalette>
#include <QVBoxLayout>

namespace {
QColor fallbackColor(const QColor &primary, const QColor &secondary, const QColor &defaultColor)
{
    if (primary.isValid()) {
        return primary;
    }
    if (secondary.isValid()) {
        return secondary;
    }
    return defaultColor;
}
}

SettingsCard::SettingsCard(QWidget *parent)
    : ThemeableWidget(parent)
    , bodyLayout(new QVBoxLayout)
{
    setObjectName("settingsCard");
    setFrameShape(QFrame::NoFrame);
    setAttribute(Qt::WA_StyledBackground, true);

    auto *outerLayout = new QVBoxLayout(this);
    outerLayout->setContentsMargins(24, 24, 24, 24);
    outerLayout->setSpacing(20);

    bodyLayout->setContentsMargins(0, 0, 0, 0);
    bodyLayout->setSpacing(24);
    outerLayout->addLayout(bodyLayout);
}

QVBoxLayout *SettingsCard::contentLayout() const
{
    return bodyLayout;
}

void SettingsCard::updateTheme()
{
    const Theme theme = ThemeManager::instance()->currentTheme();
    const QColor windowColor = palette().color(QPalette::Window);
    const QColor textColor = palette().color(QPalette::Text);

    const QColor cardBackground = fallbackColor(theme.backgroundColor, theme.background, windowColor).lighter(103);
    const QColor borderColor = fallbackColor(theme.borderColor, theme.border, theme.primary.isValid() ? theme.primary.darker(120) : textColor);
    const QColor cardText = fallbackColor(theme.textColor, theme.text, textColor);
    QColor shadowColor = theme.primary.isValid() ? theme.primary : borderColor;
    shadowColor.setAlphaF(0.25);

    setStyleSheet(QString(
        "QFrame#settingsCard {"
        "    background-color: %1;"
        "    border-radius: 18px;"
        "    border: 1px solid %2;"
        "}"
        "QFrame#settingsCard QLabel {"
        "    color: %3;"
        "}")
        .arg(cardBackground.name())
        .arg(borderColor.name())
        .arg(cardText.name()));

    auto *effect = qobject_cast<QGraphicsDropShadowEffect *>(graphicsEffect());
    if (!effect) {
        addGlowEffect(this, shadowColor, 32);
        effect = qobject_cast<QGraphicsDropShadowEffect *>(graphicsEffect());
    }
    if (effect) {
        effect->setColor(shadowColor);
        effect->setBlurRadius(32);
        effect->setOffset(0, 6);
    }
}
