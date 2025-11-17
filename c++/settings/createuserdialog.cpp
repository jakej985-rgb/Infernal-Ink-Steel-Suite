#include "createuserdialog.h"

#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QFormLayout>
#include <QLineEdit>
#include <QComboBox>
#include <QPushButton>
#include <QLabel>
#include <QMessageBox>
#include <QPalette>

#include "helper/thememanager.h"
#include "themes/theme.h"

CreateUserDialog::CreateUserDialog(const QVector<QPair<QString, QString>> &roles, QWidget *parent)
    : ThemeableDialog(parent)
    , availableRoles(roles)
{
    setWindowTitle(tr("Create User"));
    setFixedSize(360, 260);
    setupUi();
    updateTheme();
}

void CreateUserDialog::setupUi()
{
    auto *mainLayout = new QVBoxLayout(this);
    mainLayout->setContentsMargins(24, 24, 24, 24);
    mainLayout->setSpacing(18);

    auto *titleLabel = new QLabel(tr("Invite a new team member"), this);
    titleLabel->setAlignment(Qt::AlignCenter);
    titleLabel->setObjectName(QStringLiteral("create_user_title"));
    mainLayout->addWidget(titleLabel);

    usernameEdit = new QLineEdit(this);
    usernameEdit->setPlaceholderText(tr("Username"));

    passwordEdit = new QLineEdit(this);
    passwordEdit->setPlaceholderText(tr("Temporary password"));
    passwordEdit->setEchoMode(QLineEdit::Password);

    roleCombo = new QComboBox(this);
    if (availableRoles.isEmpty()) {
        roleCombo->addItem(tr("User"), QStringLiteral("User"));
    } else {
        for (const auto &role : availableRoles) {
            roleCombo->addItem(role.first, role.second);
        }
    }

    auto *form = new QFormLayout;
    form->setContentsMargins(0, 0, 0, 0);
    form->setHorizontalSpacing(16);
    form->setVerticalSpacing(12);
    form->addRow(tr("Username"), usernameEdit);
    form->addRow(tr("Password"), passwordEdit);
    form->addRow(tr("Role"), roleCombo);

    mainLayout->addLayout(form);

    auto *buttonLayout = new QHBoxLayout;
    buttonLayout->setContentsMargins(0, 0, 0, 0);
    buttonLayout->setSpacing(12);
    buttonLayout->addStretch();

    cancelBtn = new QPushButton(tr("Cancel"), this);
    createBtn = new QPushButton(tr("Create"), this);

    buttonLayout->addWidget(cancelBtn);
    buttonLayout->addWidget(createBtn);
    mainLayout->addLayout(buttonLayout);

    connect(cancelBtn, &QPushButton::clicked, this, &QDialog::reject);
    connect(createBtn, &QPushButton::clicked, this, &CreateUserDialog::handleCreate);

    fadeIn();
}

QString CreateUserDialog::username() const
{
    return usernameEdit ? usernameEdit->text().trimmed() : QString();
}

QString CreateUserDialog::password() const
{
    return passwordEdit ? passwordEdit->text() : QString();
}

QString CreateUserDialog::role() const
{
    if (!roleCombo) {
        return QString();
    }

    QString value = roleCombo->currentData().toString();
    if (value.isEmpty()) {
        value = roleCombo->currentText();
    }
    return value;
}

void CreateUserDialog::updateTheme()
{
    ThemeableDialog::updateTheme();

    const Theme theme = ThemeManager::instance()->currentTheme();
    const QColor textColor = theme.text.isValid() ? theme.text : palette().color(QPalette::WindowText);
    const QColor fieldBg = theme.background.isValid() ? theme.background.lighter(112) : palette().color(QPalette::Base);
    const QColor border = theme.borderColor.isValid() ? theme.borderColor : (theme.primary.isValid() ? theme.primary.darker(125) : textColor.darker(125));
    const QColor selection = theme.primary.isValid() ? theme.primary : textColor;
    QColor selectionText = textColor;
    if (selection == textColor) {
        selectionText = palette().color(QPalette::Base);
    }

    const QString fieldStyle = QStringLiteral(
        "QLineEdit {"
        "    background-color: %1;"
        "    border: 1px solid %2;"
        "    border-radius: 10px;"
        "    padding: 6px 10px;"
        "    color: %3;"
        "}"
        "QLineEdit:focus {"
        "    border-color: %4;"
        "}")
        .arg(fieldBg.name(QColor::HexArgb))
        .arg(border.name())
        .arg(textColor.name())
        .arg(selection.name());

    if (usernameEdit) {
        usernameEdit->setStyleSheet(fieldStyle);
    }
    if (passwordEdit) {
        passwordEdit->setStyleSheet(fieldStyle);
    }

    const QString comboStyle = QStringLiteral(
        "QComboBox {"
        "    background-color: %1;"
        "    border: 1px solid %2;"
        "    border-radius: 10px;"
        "    padding: 6px 10px;"
        "    color: %3;"
        "}"
        "QComboBox::drop-down {"
        "    width: 24px;"
        "    border: none;"
        "}"
        "QComboBox QAbstractItemView {"
        "    background-color: %1;"
        "    color: %3;"
        "    selection-background-color: %4;"
        "    selection-color: %5;"
        "    border: 1px solid %2;"
        "    border-radius: 8px;"
        "}")
        .arg(fieldBg.name(QColor::HexArgb))
        .arg(border.name())
        .arg(textColor.name())
        .arg(selection.name())
        .arg(selectionText.name());

    if (roleCombo) {
        roleCombo->setStyleSheet(comboStyle);
    }

    if (auto *titleLabel = findChild<QLabel*>(QStringLiteral("create_user_title"))) {
        titleLabel->setStyleSheet(QStringLiteral("color: %1; font-weight: 600; font-size: 16px;").arg(textColor.name()));
    }

    styleButton(createBtn);

    if (cancelBtn) {
        const QColor cancelBg = fieldBg.darker(110);
        const QColor cancelHover = cancelBg.lighter(110);
        cancelBtn->setStyleSheet(QStringLiteral(
            "QPushButton {"
            "    background-color: %1;"
            "    color: %2;"
            "    border-radius: 10px;"
            "    padding: 6px 14px;"
            "    border: 1px solid %3;"
            "}"
            "QPushButton:hover {"
            "    background-color: %4;"
            "}")
            .arg(cancelBg.name(QColor::HexArgb),
                 textColor.name(),
                 border.name(),
                 cancelHover.name(QColor::HexArgb)));
    }
}

void CreateUserDialog::handleCreate()
{
    if (username().isEmpty() || password().isEmpty()) {
        QMessageBox::warning(this, tr("Missing Details"), tr("Username and password are required."));
        return;
    }

    accept();
}
