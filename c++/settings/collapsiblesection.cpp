#include "collapsiblesection.h"
#include "helper/thememanager.h"
#include "themes/theme.h"

#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QLabel>
#include <QToolButton>
#include <QPalette>
#include <QPixmap>

CollapsibleSection::CollapsibleSection(const QString &title,
                                       const QIcon &icon,
                                       QWidget *parent)
    : ThemeableWidget(parent)
{
    setFrameShape(QFrame::NoFrame);
    setSizePolicy(QSizePolicy::Preferred, QSizePolicy::Maximum);

    auto *outerLayout = new QVBoxLayout(this);
    outerLayout->setContentsMargins(0, 0, 0, 0);
    outerLayout->setSpacing(16);

    auto *headerWidget = new QWidget(this);
    headerWidget->setObjectName("collapsibleHeader");
    auto *headerLayout = new QHBoxLayout(headerWidget);
    headerLayout->setContentsMargins(16, 14, 16, 14);
    headerLayout->setSpacing(12);

    iconLabel = new QLabel(headerWidget);
    iconLabel->setPixmap(icon.pixmap(24, 24));
    iconLabel->setFixedSize(28, 28);
    iconLabel->setAlignment(Qt::AlignCenter);

    titleLabel = new QLabel(title, headerWidget);
    QFont titleFont = titleLabel->font();
    titleFont.setPointSize(titleFont.pointSize() + 2);
    titleFont.setBold(true);
    titleLabel->setFont(titleFont);

    captionLabel = new QLabel(headerWidget);
    captionLabel->setObjectName("collapsibleCaption");
    captionLabel->setWordWrap(true);

    toggleButton = new QToolButton(headerWidget);
    toggleButton->setCheckable(true);
    toggleButton->setChecked(true);
    toggleButton->setArrowType(Qt::DownArrow);
    toggleButton->setAutoRaise(true);
    connect(toggleButton, &QToolButton::toggled, this, &CollapsibleSection::handleToggle);

    auto *titleContainer = new QVBoxLayout;
    titleContainer->setContentsMargins(0, 0, 0, 0);
    titleContainer->setSpacing(4);
    titleContainer->addWidget(titleLabel);
    titleContainer->addWidget(captionLabel);

    headerLayout->addWidget(iconLabel);
    headerLayout->addLayout(titleContainer);
    headerLayout->addStretch();
    headerLayout->addWidget(toggleButton);

    contentWidget = new QWidget(this);
    contentWidget->setObjectName("collapsibleContent");
    contentLayoutPtr = new QVBoxLayout(contentWidget);
    contentLayoutPtr->setContentsMargins(20, 12, 20, 20);
    contentLayoutPtr->setSpacing(20);

    outerLayout->addWidget(headerWidget);
    outerLayout->addWidget(contentWidget);

    updateTheme();
}

QVBoxLayout *CollapsibleSection::contentLayout() const
{
    return contentLayoutPtr;
}

void CollapsibleSection::setCaption(const QString &text)
{
    captionLabel->setText(text);
    captionLabel->setVisible(!text.trimmed().isEmpty());
}

void CollapsibleSection::setExpanded(bool expanded)
{
    toggleButton->setChecked(expanded);
    contentWidget->setVisible(expanded);
    toggleButton->setArrowType(expanded ? Qt::DownArrow : Qt::RightArrow);
}

bool CollapsibleSection::isExpanded() const
{
    return toggleButton->isChecked();
}

void CollapsibleSection::refreshTheme()
{
    updateTheme();
}

void CollapsibleSection::handleToggle(bool checked)
{
    contentWidget->setVisible(checked);
    toggleButton->setArrowType(checked ? Qt::DownArrow : Qt::RightArrow);
}

void CollapsibleSection::updateTheme()
{
    ThemeableWidget::updateTheme();

    Theme theme = ThemeManager::instance()->currentTheme();
    const QColor base = palette().color(QPalette::Window);
    const QColor text = palette().color(QPalette::WindowText);
    auto pick = [](const QColor &primary, const QColor &secondary, const QColor &fallback) {
        if (primary.isValid()) {
            return primary;
        }
        if (secondary.isValid()) {
            return secondary;
        }
        return fallback;
    };

    QColor headerBg = pick(theme.backgroundColor, theme.background, base).lighter(108);
    QColor border = pick(theme.borderColor, theme.border, base.darker(140));
    QColor caption = pick(theme.textColor, theme.text, text);
    caption.setAlphaF(0.7f);

    setStyleSheet(QStringLiteral(
        "CollapsibleSection {"
        "    background: transparent;"
        "}"
        "#collapsibleHeader {"
        "    background: %1;"
        "    border: 1px solid %2;"
        "    border-radius: 14px;"
        "}"
        "#collapsibleContent {"
        "    background: %3;"
        "    border: 1px solid %2;"
        "    border-top: none;"
        "    border-bottom-left-radius: 14px;"
        "    border-bottom-right-radius: 14px;"
        "}"
        "QLabel#collapsibleCaption {"
        "    color: %4;"
        "}")
            .arg(headerBg.name(QColor::HexArgb))
            .arg(border.name())
            .arg(headerBg.lighter(104).name(QColor::HexArgb))
            .arg(caption.name(QColor::HexArgb)));

    if (toggleButton) {
        toggleButton->setStyleSheet(QStringLiteral(
            "QToolButton {"
            "    border: none;"
            "    color: %1;"
            "}"
            "QToolButton::menu-indicator {"
            "    image: none;"
            "}")
            .arg(text.name()));
    }
}

