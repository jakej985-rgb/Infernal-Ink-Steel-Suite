#include "managertab.h"
#include "helper/thememanager.h"
#include "themes/theme.h"
#include "widgets/settingscard.h"
#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QLabel>
#include <QLineEdit>
#include <QComboBox>
#include <QPushButton>
#include <QMessageBox>
#include <QStandardItemModel>
#include <QPixmap>
#include <QPalette>
#include <QSizePolicy>
#include <algorithm>

ManagerTab::ManagerTab(UserDB &udb, QWidget *parent)
    : ThemeableWidget(parent), userDB(udb)
{
    auto *mainLayout = new QVBoxLayout(this);
    mainLayout->setContentsMargins(24, 24, 24, 24);
    mainLayout->setSpacing(24);

    managerCard = new SettingsCard(this);
    auto *cardLayout = managerCard->contentLayout();

    cardLayout->addWidget(createSectionHeader(":/icons/clients.png", tr("Select Staff")));
    cardLayout->addWidget(createCaptionLabel(tr("Load a teammate to review their access.")));

    staffEdit = new QLineEdit(managerCard);
    auto *staffLayout = new QVBoxLayout;
    staffLayout->setContentsMargins(0, 0, 0, 0);
    staffLayout->setSpacing(8);
    auto *staffLabel = new QLabel(tr("Username"), managerCard);
    bodyLabels.append(staffLabel);
    staffLayout->addWidget(staffLabel);
    staffLayout->addWidget(staffEdit);
    cardLayout->addLayout(staffLayout);

    cardLayout->addWidget(createSectionHeader(":/icons/stats.png", tr("Assign Roles")));
    cardLayout->addWidget(createCaptionLabel(tr("Check the responsibilities this staff member should have.")));

    rolesCombo = new QComboBox(managerCard);
    QStandardItemModel *model = new QStandardItemModel();
    for (const QString &roleName : allRoles()) {
        QStandardItem *item = new QStandardItem(roleName);
        item->setFlags(Qt::ItemIsUserCheckable | Qt::ItemIsEnabled);
        item->setData(Qt::Unchecked, Qt::CheckStateRole);
        model->appendRow(item);
    }
    rolesCombo->setModel(model);
    auto *rolesLayout = new QVBoxLayout;
    rolesLayout->setContentsMargins(0, 0, 0, 0);
    rolesLayout->setSpacing(8);
    auto *rolesLabel = new QLabel(tr("Roles"), managerCard);
    bodyLabels.append(rolesLabel);
    rolesLayout->addWidget(rolesLabel);
    rolesLayout->addWidget(rolesCombo);
    cardLayout->addLayout(rolesLayout);

    updateBtn = new QPushButton(tr("Update Roles"), managerCard);
    auto *buttonLayout = new QHBoxLayout;
    buttonLayout->setContentsMargins(0, 0, 0, 0);
    buttonLayout->setSpacing(12);
    buttonLayout->addStretch();
    buttonLayout->addWidget(updateBtn);
    cardLayout->addLayout(buttonLayout);

    connect(staffEdit, &QLineEdit::editingFinished, this, [=]() {
        QString uname = staffEdit->text().trimmed();
        if (uname.isEmpty()) return;

        User u = userDB.getUserByUsername(uname);
        QStringList currentRoles = splitRoles(u.role);

        for (int i = 0; i < model->rowCount(); ++i) {
            QStandardItem *item = model->item(i);
            item->setCheckState(currentRoles.contains(item->text()) ? Qt::Checked : Qt::Unchecked);
        }
    });

    connect(updateBtn, &QPushButton::clicked, this, [=]() {
        QString uname = staffEdit->text().trimmed();
        if (uname.isEmpty()) return;

        QStringList selectedRoles;
        for (int i = 0; i < model->rowCount(); ++i) {
            QStandardItem *item = model->item(i);
            if (item->checkState() == Qt::Checked) selectedRoles.append(item->text());
        }

        QString roleStr = selectedRoles.isEmpty() ? "User" : selectedRoles.join("|");

        if (!userDB.updateRole(uname, roleStr)) {
            QMessageBox::critical(this, tr("Error"), tr("Failed to update roles. Check username."));
            return;
        }
        QMessageBox::information(this, tr("Updated"), uname + tr("'s roles updated."));
    });

    mainLayout->addWidget(managerCard);
    mainLayout->addStretch();
    updateTheme();
}

void ManagerTab::updateTheme()
{
    Theme t = ThemeManager::instance()->currentTheme();
    const QColor defaultText = palette().color(QPalette::Text);
    const QColor textColor = t.textColor.isValid() ? t.textColor : (t.text.isValid() ? t.text : defaultText);
    QColor mutedText = textColor;
    mutedText.setAlphaF(0.75);
    const QColor accent = t.primary.isValid() ? t.primary : (t.accent.isValid() ? t.accent : textColor);
    const QColor fieldBg = t.background.isValid() ? t.background.lighter(110) : palette().color(QPalette::Base);
    const QColor border = t.borderColor.isValid() ? t.borderColor : (t.border.isValid() ? t.border : accent.darker(115));

    for (QLabel *heading : headingLabels) {
        heading->setStyleSheet(QStringLiteral("color: %1; font-weight: 600;").arg(textColor.name()));
    }

    for (QLabel *caption : captionLabels) {
        caption->setStyleSheet(QStringLiteral("color: %1;").arg(mutedText.name(QColor::HexArgb)));
    }

    for (QLabel *body : bodyLabels) {
        body->setStyleSheet(QStringLiteral("color: %1; font-weight: 500;").arg(textColor.name()));
    }

    const QString fieldStyle = QStringLiteral(
        "QLineEdit, QComboBox {"
        "    background-color: %1;"
        "    border: 1px solid %2;"
        "    border-radius: 10px;"
        "    padding: 6px 10px;"
        "    color: %3;"
        "}"
        "QLineEdit:focus, QComboBox:focus {"
        "    border-color: %4;"
        "}")
        .arg(fieldBg.name(QColor::HexArgb))
        .arg(border.name())
        .arg(textColor.name())
        .arg(accent.name());

    staffEdit->setStyleSheet(fieldStyle);
    rolesCombo->setStyleSheet(fieldStyle);

    styleButton(updateBtn);
    if (managerCard) {
        managerCard->updateTheme();
    }
}

QStringList ManagerTab::splitRoles(const QString &roleStr)
{
    return roleStr.isEmpty() ? QStringList() : roleStr.split("|", Qt::SkipEmptyParts);
}

QStringList ManagerTab::allRoles()
{
    return {"Admin", "Manager", "Tattoo", "Piercer", "User"};
}

QWidget *ManagerTab::createSectionHeader(const QString &iconPath, const QString &title)
{
    QWidget *header = new QWidget(this);
    auto *layout = new QHBoxLayout(header);
    layout->setContentsMargins(0, 0, 0, 0);
    layout->setSpacing(8);

    auto *iconLabel = new QLabel(header);
    iconLabel->setPixmap(QPixmap(iconPath).scaled(24, 24, Qt::KeepAspectRatio, Qt::SmoothTransformation));

    auto *titleLabel = new QLabel(title, header);
    QFont font = titleLabel->font();
    font.setPointSize(font.pointSize() + 2);
    font.setBold(true);
    titleLabel->setFont(font);
    titleLabel->setSizePolicy(QSizePolicy::Preferred, QSizePolicy::Preferred);

    layout->addWidget(iconLabel);
    layout->addWidget(titleLabel);
    layout->addStretch();

    headingLabels.append(titleLabel);
    return header;
}

QLabel *ManagerTab::createCaptionLabel(const QString &text)
{
    auto *caption = new QLabel(text, this);
    caption->setWordWrap(true);
    QFont font = caption->font();
    const int pointSize = font.pointSize();
    if (pointSize > 0) {
        font.setPointSize(std::max(9, pointSize - 1));
    }
    caption->setFont(font);
    captionLabels.append(caption);
    return caption;
}
